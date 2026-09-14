using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Mathematics.Graphs;

internal class Graph<TNode, TEdge> : IGraph<TNode, TEdge> {

	private readonly IEdgeFactory<TNode, TEdge> _edgeFactory;
	private readonly List<INode<TNode, TEdge>> _nodes;
	private readonly List<IEdge<TNode, TEdge>> _edges;
	private readonly Dictionary<INode<TNode, TEdge>, List<IEdge<TNode, TEdge>>> _adjacency;

	public Graph(
		IEdgeFactory<TNode, TEdge> edgeFactory
	) {
		ArgumentNullException.ThrowIfNull( edgeFactory );
		_edgeFactory = edgeFactory;
		_nodes = [];
		_edges = [];
		_adjacency = [];
	}

	IReadOnlyList<INode<TNode, TEdge>> IGraph<TNode, TEdge>.Nodes => _nodes;

	IReadOnlyList<IEdge<TNode, TEdge>> IGraph<TNode, TEdge>.Edges => _edges;

	IReadOnlyList<IEdge<TNode, TEdge>> IGraph<TNode, TEdge>.GetEdgesFor(
		INode<TNode, TEdge> node
	) {
		return DoGetEdgesFor( node );
	}

	IReadOnlyList<INode<TNode, TEdge>> IGraph<TNode, TEdge>.GetConnectedTo(
		INode<TNode, TEdge> node
	) {
		IReadOnlyList<IEdge<TNode, TEdge>> edges = DoGetEdgesFor( node );
		List<INode<TNode, TEdge>> connected = new List<INode<TNode, TEdge>>( edges.Count );
		foreach( IEdge<TNode, TEdge> edge in edges ) {
			connected.Add( ReferenceEquals( edge.A, node ) ? edge.B : edge.A );
		}

		return connected;
	}

	void IGraph<TNode, TEdge>.AddNode(
		INode<TNode, TEdge> node
	) {
		ArgumentNullException.ThrowIfNull( node );
		if( _nodes.Contains( node ) ) {
			return;
		}
		_nodes.Add( node );
		_adjacency[node] = [];
	}

	bool IGraph<TNode, TEdge>.RemoveNode(
		INode<TNode, TEdge> node
	) {
		ArgumentNullException.ThrowIfNull( node );
		if( !_nodes.Remove( node ) ) {
			return false;
		}

		List<IEdge<TNode, TEdge>> incident = _adjacency[node];
		foreach( IEdge<TNode, TEdge> edge in incident ) {
			_ = _edges.Remove( edge );
			INode<TNode, TEdge> other = ReferenceEquals( edge.A, node ) ? edge.B : edge.A;
			_ = _adjacency[other].Remove( edge );
		}

		_ = _adjacency.Remove( node );

		return true;
	}

	bool IGraph<TNode, TEdge>.TryConnectNodes(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		TEdge? edgeContext,
		out IEdge<TNode, TEdge> edge
	) {
		return DoTryConnect(
			a,
			b,
			( x, y ) => _edgeFactory.Create( edgeContext, x, y ),
			out edge
		);
	}

	bool IGraph<TNode, TEdge>.TryDisconnectNodes(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b
	) {
		ArgumentNullException.ThrowIfNull( a );
		ArgumentNullException.ThrowIfNull( b );

		if( !TryGetEdge( a, b, out IEdge<TNode, TEdge>? edge ) ) {
			return false;
		}

		_ = _edges.Remove( edge );
		_ = _adjacency[a].Remove( edge );
		_ = _adjacency[b].Remove( edge );

		return true;
	}

	bool IGraph<TNode, TEdge>.TryGetDistance(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		out int distance
	) {
		return DoTryGetDistance( a, b, out distance );
	}

	protected bool DoTryGetDistance(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		out int distance
	) {
		ArgumentNullException.ThrowIfNull( a );
		ArgumentNullException.ThrowIfNull( b );

		distance = 0;
		if( !ContainsNode( a ) || !ContainsNode( b ) ) {
			return false;
		}

		if( ReferenceEquals( a, b ) ) {
			return true;
		}

		HashSet<INode<TNode, TEdge>> visited = [a];
		Queue<(INode<TNode, TEdge> Node, int Depth)> queue = new Queue<(INode<TNode, TEdge>, int)>();
		queue.Enqueue( (a, 0) );

		while( queue.Count > 0 ) {
			( INode<TNode, TEdge> node, int depth ) = queue.Dequeue();
			foreach( IEdge<TNode, TEdge> edge in _adjacency[node] ) {
				INode<TNode, TEdge> neighbour = ReferenceEquals( edge.A, node ) ? edge.B : edge.A;
				if( !visited.Add( neighbour ) ) {
					continue;
				}
				if( ReferenceEquals( neighbour, b ) ) {
					distance = depth + 1;
					return true;
				}
				queue.Enqueue( (neighbour, depth + 1) );
			}
		}

		return false;
	}

	bool IGraph<TNode, TEdge>.TryGetPath(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		out IReadOnlyList<INode<TNode, TEdge>> path
	) {
		return DoTryGetPath( a, b, out path );
	}

	bool IGraph<TNode, TEdge>.TryVisitPath(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		Action<INode<TNode, TEdge>> visitor
	) {
		return DoTryVisitPath( a, b, visitor );
	}

	bool IGraph<TNode, TEdge>.IsConnectedTo(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b
	) {
		return DoIsConnectedTo( a, b );
	}

	protected bool DoIsConnectedTo(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b
	) {
		ArgumentNullException.ThrowIfNull( a );
		ArgumentNullException.ThrowIfNull( b );

		if( !ContainsNode( a ) || !ContainsNode( b ) ) {
			return false;
		}

		if( ReferenceEquals( a, b ) ) {
			return true;
		}

		HashSet<INode<TNode, TEdge>> visited = [a];
		Queue<INode<TNode, TEdge>> queue = new Queue<INode<TNode, TEdge>>();
		queue.Enqueue( a );

		while( queue.Count > 0 ) {
			INode<TNode, TEdge> node = queue.Dequeue();
			foreach( IEdge<TNode, TEdge> edge in _adjacency[node] ) {
				INode<TNode, TEdge> neighbour = ReferenceEquals( edge.A, node ) ? edge.B : edge.A;
				if( !visited.Add( neighbour ) ) {
					continue;
				}
				if( ReferenceEquals( neighbour, b ) ) {
					return true;
				}
				queue.Enqueue( neighbour );
			}
		}

		return false;
	}

	protected bool DoTryVisitPath(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		Action<INode<TNode, TEdge>> visitor
	) {
		ArgumentNullException.ThrowIfNull( visitor );
		if( !DoTryGetPath( a, b, out IReadOnlyList<INode<TNode, TEdge>> path ) ) {
			return false;
		}

		foreach( INode<TNode, TEdge> node in path ) {
			visitor( node );
		}

		return true;
	}

	private bool DoTryGetPath(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		out IReadOnlyList<INode<TNode, TEdge>> path
	) {
		ArgumentNullException.ThrowIfNull( a );
		ArgumentNullException.ThrowIfNull( b );

		path = [];
		if( !ContainsNode( a ) || !ContainsNode( b ) ) {
			return false;
		}

		if( ReferenceEquals( a, b ) ) {
			path = [a];
			return true;
		}

		Dictionary<INode<TNode, TEdge>, INode<TNode, TEdge>> cameFrom = [];
		HashSet<INode<TNode, TEdge>> visited = [a];
		Queue<INode<TNode, TEdge>> queue = new Queue<INode<TNode, TEdge>>();
		queue.Enqueue( a );

		while( queue.Count > 0 ) {
			INode<TNode, TEdge> node = queue.Dequeue();
			foreach( IEdge<TNode, TEdge> edge in _adjacency[node] ) {
				INode<TNode, TEdge> neighbour = ReferenceEquals( edge.A, node ) ? edge.B : edge.A;
				if( !visited.Add( neighbour ) ) {
					continue;
				}
				cameFrom[neighbour] = node;
				if( ReferenceEquals( neighbour, b ) ) {
					path = BuildPath( cameFrom, a, b );
					return true;
				}
				queue.Enqueue( neighbour );
			}
		}

		return false;
	}

	protected static IReadOnlyList<INode<TNode, TEdge>> BuildPath(
		Dictionary<INode<TNode, TEdge>, INode<TNode, TEdge>> cameFrom,
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b
	) {
		List<INode<TNode, TEdge>> path = [b];
		INode<TNode, TEdge> current = b;
		while( !ReferenceEquals( current, a ) ) {
			current = cameFrom[current];
			path.Add( current );
		}
		path.Reverse();

		return path;
	}

	protected IReadOnlyList<IEdge<TNode, TEdge>> DoGetEdgesFor(
		INode<TNode, TEdge> node
	) {
		ArgumentNullException.ThrowIfNull( node );
		return _adjacency.TryGetValue( node, out List<IEdge<TNode, TEdge>>? edges ) ? edges : [];
	}

	protected bool ContainsNode(
		INode<TNode, TEdge> node
	) {
		return _adjacency.ContainsKey( node );
	}

	protected bool DoTryConnect(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		Func<INode<TNode, TEdge>, INode<TNode, TEdge>, IEdge<TNode, TEdge>> factory,
		[NotNullWhen( true )] out IEdge<TNode, TEdge> edge
	) {
		ArgumentNullException.ThrowIfNull( a );
		ArgumentNullException.ThrowIfNull( b );

		edge = default!;
		if( ReferenceEquals( a, b )
			|| !ContainsNode( a )
			|| !ContainsNode( b )
			|| TryGetEdge( a, b, out _ )
		) {
			return false;
		}

		IEdge<TNode, TEdge> newEdge = factory( a, b );
		_edges.Add( newEdge );
		_adjacency[a].Add( newEdge );
		_adjacency[b].Add( newEdge );
		edge = newEdge;

		return true;
	}

	private bool TryGetEdge(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		[NotNullWhen( true )] out IEdge<TNode, TEdge>? edge
	) {
		if( _adjacency.TryGetValue( a, out List<IEdge<TNode, TEdge>>? incident ) ) {
			foreach( IEdge<TNode, TEdge> candidate in incident ) {
				if( ( ReferenceEquals( candidate.A, a ) && ReferenceEquals( candidate.B, b ) )
					|| ( ReferenceEquals( candidate.A, b ) && ReferenceEquals( candidate.B, a ) )
				) {
					edge = candidate;
					return true;
				}
			}
		}
		edge = default;
		return false;
	}
}
