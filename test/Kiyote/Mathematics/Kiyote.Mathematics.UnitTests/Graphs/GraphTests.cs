using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Mathematics.Graphs.UnitTests;

[TestFixture]
[ExcludeFromCodeCoverage]
internal sealed class GraphTests {

	private IGraph<string, string> _graph;
	private INode<string, string> _a;
	private INode<string, string> _b;
	private INode<string, string> _c;

	[SetUp]
	public void SetUp() {
		_graph = new Graph<string, string>( new EdgeFactory<string, string>() );
		_a = new Node<string, string>( "a" );
		_b = new Node<string, string>( "b" );
		_c = new Node<string, string>( "c" );
	}

	[Test]
	public void Ctor_NullEdgeFactory_Throws() {
		Assert.That(
			() => new Graph<string, string>( null! ),
			Throws.ArgumentNullException
		);
	}

	[Test]
	public void AddNode_NewNode_IsAdded() {
		_graph.AddNode( _a );

		Assert.That( _graph.Nodes, Is.EqualTo( [_a] ) );
	}

	[Test]
	public void AddNode_SameNodeTwice_IsAddedOnce() {
		_graph.AddNode( _a );
		_graph.AddNode( _a );

		Assert.That( _graph.Nodes, Has.Count.EqualTo( 1 ) );
	}

	[Test]
	public void AddNode_Null_Throws() {
		Assert.That( () => _graph.AddNode( null! ), Throws.ArgumentNullException );
	}

	[Test]
	public void RemoveNode_UnknownNode_ReturnsFalse() {
		Assert.That( _graph.RemoveNode( _a ), Is.False );
	}

	[Test]
	public void RemoveNode_KnownNode_ReturnsTrueAndRemoves() {
		_graph.AddNode( _a );

		Assert.That( _graph.RemoveNode( _a ), Is.True );
		Assert.That( _graph.Nodes, Is.Empty );
	}

	[Test]
	public void RemoveNode_NodeWithEdges_RemovesIncidentEdges() {
		_graph.AddNode( _a );
		_graph.AddNode( _b );
		_ = _graph.TryConnectNodes( _a, _b, "edge", out _ );

		_ = _graph.RemoveNode( _a );

		Assert.That( _graph.Edges, Is.Empty );
		Assert.That( _graph.GetEdgesFor( _b ), Is.Empty );
	}

	[Test]
	public void TryConnectNodes_KnownNodes_CreatesEdge() {
		_graph.AddNode( _a );
		_graph.AddNode( _b );

		Assert.That( _graph.TryConnectNodes( _a, _b, "edge", out IEdge<string, string> edge ), Is.True );
		Assert.That( edge.Context, Is.EqualTo( "edge" ) );
		Assert.That( edge.A, Is.SameAs( _a ) );
		Assert.That( edge.B, Is.SameAs( _b ) );
		Assert.That( _graph.Edges, Is.EqualTo( [edge] ) );
		Assert.That( _graph.GetEdgesFor( _a ), Is.EqualTo( [edge] ) );
		Assert.That( _graph.GetEdgesFor( _b ), Is.EqualTo( [edge] ) );
	}

	[Test]
	public void TryConnectNodes_SameNode_ReturnsFalse() {
		_graph.AddNode( _a );

		Assert.That( _graph.TryConnectNodes( _a, _a, "edge", out _ ), Is.False );
		Assert.That( _graph.Edges, Is.Empty );
	}

	[Test]
	public void TryConnectNodes_UnknownNode_ReturnsFalse() {
		_graph.AddNode( _a );

		Assert.That( _graph.TryConnectNodes( _a, _b, "edge", out _ ), Is.False );
	}

	[Test]
	public void TryConnectNodes_AlreadyConnected_ReturnsFalse() {
		_graph.AddNode( _a );
		_graph.AddNode( _b );
		_ = _graph.TryConnectNodes( _a, _b, "edge", out _ );

		Assert.That( _graph.TryConnectNodes( _b, _a, "other", out _ ), Is.False );
		Assert.That( _graph.Edges, Has.Count.EqualTo( 1 ) );
	}

	[Test]
	public void TryConnectNodes_NullNode_Throws() {
		Assert.That(
			() => _graph.TryConnectNodes( null!, _b, "edge", out _ ),
			Throws.ArgumentNullException
		);
	}

	[Test]
	public void TryDisconnectNodes_ConnectedNodes_ReturnsTrueAndRemovesEdge() {
		_graph.AddNode( _a );
		_graph.AddNode( _b );
		_ = _graph.TryConnectNodes( _a, _b, "edge", out _ );

		Assert.That( _graph.TryDisconnectNodes( _a, _b ), Is.True );
		Assert.That( _graph.Edges, Is.Empty );
		Assert.That( _graph.GetEdgesFor( _a ), Is.Empty );
		Assert.That( _graph.GetEdgesFor( _b ), Is.Empty );
	}

	[Test]
	public void TryDisconnectNodes_NotConnected_ReturnsFalse() {
		_graph.AddNode( _a );
		_graph.AddNode( _b );

		Assert.That( _graph.TryDisconnectNodes( _a, _b ), Is.False );
	}

	[Test]
	public void TryDisconnectNodes_NullNode_Throws() {
		Assert.That(
			() => _graph.TryDisconnectNodes( _a, null! ),
			Throws.ArgumentNullException
		);
	}

	[Test]
	public void GetEdgesFor_NodeWithoutEdges_ReturnsEmpty() {
		_graph.AddNode( _a );

		Assert.That( _graph.GetEdgesFor( _a ), Is.Empty );
	}

	[Test]
	public void GetEdgesFor_UnknownNode_ReturnsEmpty() {
		Assert.That( _graph.GetEdgesFor( _a ), Is.Empty );
	}

	[Test]
	public void GetEdgesFor_Null_Throws() {
		Assert.That( () => _graph.GetEdgesFor( null! ), Throws.ArgumentNullException );
	}

	[Test]
	public void GetConnectedTo_ConnectedNodes_ReturnsNeighbours() {
		_graph.AddNode( _a );
		_graph.AddNode( _b );
		_graph.AddNode( _c );
		_ = _graph.TryConnectNodes( _a, _b, "ab", out _ );
		_ = _graph.TryConnectNodes( _c, _a, "ca", out _ );

		Assert.That( _graph.GetConnectedTo( _a ), Is.EquivalentTo( [_b, _c] ) );
	}

	[Test]
	public void GetConnectedTo_NodeWithoutEdges_ReturnsEmpty() {
		_graph.AddNode( _a );

		Assert.That( _graph.GetConnectedTo( _a ), Is.Empty );
	}

	[Test]
	public void GetConnectedTo_UnknownNode_ReturnsEmpty() {
		Assert.That( _graph.GetConnectedTo( _a ), Is.Empty );
	}

	[Test]
	public void GetConnectedTo_Null_Throws() {
		Assert.That( () => _graph.GetConnectedTo( null! ), Throws.ArgumentNullException );
	}

	[Test]
	public void TryGetDistance_SameNode_ReturnsZero() {
		_graph.AddNode( _a );

		Assert.That( _graph.TryGetDistance( _a, _a, out int distance ), Is.True );
		Assert.That( distance, Is.Zero );
	}

	[Test]
	public void TryGetDistance_AdjacentNodes_ReturnsOne() {
		_graph.AddNode( _a );
		_graph.AddNode( _b );
		_ = _graph.TryConnectNodes( _a, _b, "edge", out _ );

		Assert.That( _graph.TryGetDistance( _a, _b, out int distance ), Is.True );
		Assert.That( distance, Is.EqualTo( 1 ) );
	}

	[Test]
	public void TryGetDistance_TwoHops_ReturnsTwo() {
		_graph.AddNode( _a );
		_graph.AddNode( _b );
		_graph.AddNode( _c );
		_ = _graph.TryConnectNodes( _a, _b, "ab", out _ );
		_ = _graph.TryConnectNodes( _b, _c, "bc", out _ );

		Assert.That( _graph.TryGetDistance( _a, _c, out int distance ), Is.True );
		Assert.That( distance, Is.EqualTo( 2 ) );
	}

	[Test]
	public void TryGetDistance_Disconnected_ReturnsFalse() {
		_graph.AddNode( _a );
		_graph.AddNode( _b );

		Assert.That( _graph.TryGetDistance( _a, _b, out _ ), Is.False );
	}

	[Test]
	public void TryGetDistance_UnknownNode_ReturnsFalse() {
		_graph.AddNode( _a );

		Assert.That( _graph.TryGetDistance( _a, _b, out _ ), Is.False );
	}

	[Test]
	public void TryGetDistance_NullNode_Throws() {
		Assert.That(
			() => _graph.TryGetDistance( null!, _b, out _ ),
			Throws.ArgumentNullException
		);
	}

	[Test]
	public void TryGetPath_SameNode_ReturnsNodeOnly() {
		_graph.AddNode( _a );

		Assert.That( _graph.TryGetPath( _a, _a, out IReadOnlyList<INode<string, string>> path ), Is.True );
		Assert.That( path, Is.EqualTo( [_a] ) );
	}

	[Test]
	public void TryGetPath_AdjacentNodes_ReturnsBothNodes() {
		_graph.AddNode( _a );
		_graph.AddNode( _b );
		_ = _graph.TryConnectNodes( _a, _b, "ab", out _ );

		Assert.That( _graph.TryGetPath( _a, _b, out IReadOnlyList<INode<string, string>> path ), Is.True );
		Assert.That( path, Is.EqualTo( [_a, _b] ) );
	}

	[Test]
	public void TryGetPath_TwoHops_ReturnsNodesInOrder() {
		_graph.AddNode( _a );
		_graph.AddNode( _b );
		_graph.AddNode( _c );
		_ = _graph.TryConnectNodes( _a, _b, "ab", out _ );
		_ = _graph.TryConnectNodes( _b, _c, "bc", out _ );

		Assert.That( _graph.TryGetPath( _a, _c, out IReadOnlyList<INode<string, string>> path ), Is.True );
		Assert.That( path, Is.EqualTo( [_a, _b, _c] ) );
	}

	[Test]
	public void TryGetPath_Disconnected_ReturnsFalse() {
		_graph.AddNode( _a );
		_graph.AddNode( _b );

		Assert.That( _graph.TryGetPath( _a, _b, out IReadOnlyList<INode<string, string>> path ), Is.False );
		Assert.That( path, Is.Empty );
	}

	[Test]
	public void TryGetPath_UnknownNode_ReturnsFalse() {
		_graph.AddNode( _a );

		Assert.That( _graph.TryGetPath( _a, _b, out _ ), Is.False );
	}

	[Test]
	public void TryGetPath_NullNode_Throws() {
		Assert.That(
			() => _graph.TryGetPath( null!, _b, out _ ),
			Throws.ArgumentNullException
		);
	}

	[Test]
	public void TryVisitPath_SameNode_VisitsNodeOnly() {
		_graph.AddNode( _a );
		List<INode<string, string>> visited = [];

		Assert.That( _graph.TryVisitPath( _a, _a, visited.Add ), Is.True );
		Assert.That( visited, Is.EqualTo( [_a] ) );
	}

	[Test]
	public void TryVisitPath_TwoHops_VisitsNodesInOrder() {
		_graph.AddNode( _a );
		_graph.AddNode( _b );
		_graph.AddNode( _c );
		_ = _graph.TryConnectNodes( _a, _b, "ab", out _ );
		_ = _graph.TryConnectNodes( _b, _c, "bc", out _ );
		List<INode<string, string>> visited = [];

		Assert.That( _graph.TryVisitPath( _a, _c, visited.Add ), Is.True );
		Assert.That( visited, Is.EqualTo( [_a, _b, _c] ) );
	}

	[Test]
	public void TryVisitPath_Disconnected_ReturnsFalseAndDoesNotVisit() {
		_graph.AddNode( _a );
		_graph.AddNode( _b );
		List<INode<string, string>> visited = [];

		Assert.That( _graph.TryVisitPath( _a, _b, visited.Add ), Is.False );
		Assert.That( visited, Is.Empty );
	}

	[Test]
	public void TryVisitPath_UnknownNode_ReturnsFalseAndDoesNotVisit() {
		_graph.AddNode( _a );
		List<INode<string, string>> visited = [];

		Assert.That( _graph.TryVisitPath( _a, _b, visited.Add ), Is.False );
		Assert.That( visited, Is.Empty );
	}

	[Test]
	public void TryVisitPath_NullVisitor_Throws() {
		_graph.AddNode( _a );

		Assert.That(
			() => _graph.TryVisitPath( _a, _a, null! ),
			Throws.ArgumentNullException
		);
	}

	[Test]
	public void TryVisitPath_NullNode_Throws() {
		Assert.That(
			() => _graph.TryVisitPath( null!, _b, _ => { } ),
			Throws.ArgumentNullException
		);
	}

	[Test]
	public void IsConnectedTo_SameNode_ReturnsTrue() {
		_graph.AddNode( _a );

		Assert.That( _graph.IsConnectedTo( _a, _a ), Is.True );
	}

	[Test]
	public void IsConnectedTo_AdjacentNodes_ReturnsTrue() {
		_graph.AddNode( _a );
		_graph.AddNode( _b );
		_ = _graph.TryConnectNodes( _a, _b, "edge", out _ );

		Assert.That( _graph.IsConnectedTo( _a, _b ), Is.True );
	}

	[Test]
	public void IsConnectedTo_TwoHops_ReturnsTrue() {
		_graph.AddNode( _a );
		_graph.AddNode( _b );
		_graph.AddNode( _c );
		_ = _graph.TryConnectNodes( _a, _b, "ab", out _ );
		_ = _graph.TryConnectNodes( _b, _c, "bc", out _ );

		Assert.That( _graph.IsConnectedTo( _a, _c ), Is.True );
	}

	[Test]
	public void IsConnectedTo_Disconnected_ReturnsFalse() {
		_graph.AddNode( _a );
		_graph.AddNode( _b );

		Assert.That( _graph.IsConnectedTo( _a, _b ), Is.False );
	}

	[Test]
	public void IsConnectedTo_UnknownNode_ReturnsFalse() {
		_graph.AddNode( _a );

		Assert.That( _graph.IsConnectedTo( _a, _b ), Is.False );
	}

	[Test]
	public void IsConnectedTo_NullNode_Throws() {
		Assert.That(
			() => _graph.IsConnectedTo( null!, _b ),
			Throws.ArgumentNullException
		);
	}
}
