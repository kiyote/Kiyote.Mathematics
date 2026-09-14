namespace Kiyote.Mathematics.Graphs;

internal sealed class WeightedGraph<TNode, TEdge> : Graph<TNode, TEdge>, IWeightedGraph<TNode, TEdge> {

	private readonly IWeightedEdgeFactory<TNode, TEdge> _weightedEdgeFactory;

	public WeightedGraph(
		IWeightedEdgeFactory<TNode, TEdge> weightedEdgeFactory
	) : base( weightedEdgeFactory ) {
		_weightedEdgeFactory = weightedEdgeFactory;
	}

	bool IWeightedGraph<TNode, TEdge>.TryConnectNodes(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		TEdge? edgeContext,
		int weight,
		out IWeightedEdge<TNode, TEdge> edge
	) {
		ArgumentOutOfRangeException.ThrowIfNegative( weight );

		if( !DoTryConnect(
			a,
			b,
			( x, y ) => _weightedEdgeFactory.Create( edgeContext, x, y, weight ),
			out IEdge<TNode, TEdge> newEdge
		) ) {
			edge = default!;
			return false;
		}

		edge = (IWeightedEdge<TNode, TEdge>)newEdge;
		return true;
	}

	bool IWeightedGraph<TNode, TEdge>.TryGetWeightedDistance(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		out int distance
	) {
		return DoTryGetWeightedDistance( a, b, out distance );
	}

	private bool DoTryGetWeightedDistance(
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

		Dictionary<INode<TNode, TEdge>, int> best = new Dictionary<INode<TNode, TEdge>, int>() {
			[a] = 0
		};
		PriorityQueue<INode<TNode, TEdge>, int> queue = new PriorityQueue<INode<TNode, TEdge>, int>();
		queue.Enqueue( a, 0 );

		while( queue.TryDequeue( out INode<TNode, TEdge>? node, out int cost ) ) {
			if( ReferenceEquals( node, b ) ) {
				distance = cost;
				return true;
			}

			if( cost > best[node] ) {
				continue;
			}

			foreach( IEdge<TNode, TEdge> edge in DoGetEdgesFor( node ) ) {
				INode<TNode, TEdge> neighbour = ReferenceEquals( edge.A, node ) ? edge.B : edge.A;
				int candidate = cost + GetWeight( edge );
				if( best.TryGetValue( neighbour, out int existing )
					&& existing <= candidate
				) {
					continue;
				}
				best[neighbour] = candidate;
				queue.Enqueue( neighbour, candidate );
			}
		}

		return false;
	}

	bool IWeightedGraph<TNode, TEdge>.TryGetWeightedPath(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		out IReadOnlyList<INode<TNode, TEdge>> path
	) {
		return DoTryGetWeightedPath( a, b, out path );
	}

	bool IWeightedGraph<TNode, TEdge>.TryVisitWeightedPath(
		INode<TNode, TEdge> a,
		INode<TNode, TEdge> b,
		Action<INode<TNode, TEdge>> visitor
	) {
		ArgumentNullException.ThrowIfNull( visitor );
		if( !DoTryGetWeightedPath( a, b, out IReadOnlyList<INode<TNode, TEdge>> path ) ) {
			return false;
		}

		foreach( INode<TNode, TEdge> node in path ) {
			visitor( node );
		}

		return true;
	}

	private bool DoTryGetWeightedPath(
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
		Dictionary<INode<TNode, TEdge>, int> best = new Dictionary<INode<TNode, TEdge>, int>() {
			[a] = 0
		};
		PriorityQueue<INode<TNode, TEdge>, int> queue = new PriorityQueue<INode<TNode, TEdge>, int>();
		queue.Enqueue( a, 0 );

		while( queue.TryDequeue( out INode<TNode, TEdge>? node, out int cost ) ) {
			if( ReferenceEquals( node, b ) ) {
				path = BuildPath( cameFrom, a, b );
				return true;
			}

			if( cost > best[node] ) {
				continue;
			}

			foreach( IEdge<TNode, TEdge> edge in DoGetEdgesFor( node ) ) {
				INode<TNode, TEdge> neighbour = ReferenceEquals( edge.A, node ) ? edge.B : edge.A;
				int candidate = cost + GetWeight( edge );
				if( best.TryGetValue( neighbour, out int existing )
					&& existing <= candidate
				) {
					continue;
				}
				best[neighbour] = candidate;
				cameFrom[neighbour] = node;
				queue.Enqueue( neighbour, candidate );
			}
		}

		return false;
	}

	private static int GetWeight(
		IEdge<TNode, TEdge> edge
	) {
		return edge is IWeightedEdge<TNode, TEdge> weighted ? weighted.Weight : 1;
	}
}
