using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Mathematics.Graphs.UnitTests;

[TestFixture]
[ExcludeFromCodeCoverage]
internal sealed class WeightedGraphTests {

	private IWeightedGraph<string, string> _graph;
	private INode<string, string> _a;
	private INode<string, string> _b;
	private INode<string, string> _c;

	[SetUp]
	public void SetUp() {
		_graph = new WeightedGraph<string, string>( new WeightedEdgeFactory<string, string>() );
		_a = new Node<string, string>( "a" );
		_b = new Node<string, string>( "b" );
		_c = new Node<string, string>( "c" );
		_graph.AddNode( _a );
		_graph.AddNode( _b );
		_graph.AddNode( _c );
	}

	[Test]
	public void TryConnectNodes_KnownNodes_CreatesWeightedEdge() {
		Assert.That(
			_graph.TryConnectNodes( _a, _b, "edge", 5, out IWeightedEdge<string, string> edge ),
			Is.True
		);
		Assert.That( edge.Weight, Is.EqualTo( 5 ) );
		Assert.That( edge.Context, Is.EqualTo( "edge" ) );
		Assert.That( _graph.Edges, Is.EqualTo( [edge] ) );
	}

	[Test]
	public void TryConnectNodes_AlreadyConnected_ReturnsFalse() {
		_ = _graph.TryConnectNodes( _a, _b, "edge", 5, out _ );

		Assert.That( _graph.TryConnectNodes( _a, _b, "other", 1, out _ ), Is.False );
	}

	[Test]
	public void TryConnectNodes_NegativeWeight_Throws() {
		Assert.That(
			() => _graph.TryConnectNodes( _a, _b, "edge", -1, out _ ),
			Throws.TypeOf<ArgumentOutOfRangeException>()
		);
	}

	[Test]
	public void TryConnectNodes_Unweighted_CreatesEdgeOfWeightOne() {
		_ = _graph.TryConnectNodes( _a, _b, "edge", out IEdge<string, string> edge );

		Assert.That( edge, Is.InstanceOf<IWeightedEdge<string, string>>() );
		Assert.That( ( (IWeightedEdge<string, string>)edge ).Weight, Is.EqualTo( 1 ) );
	}

	[Test]
	public void TryGetWeightedDistance_SameNode_ReturnsZero() {
		Assert.That( _graph.TryGetWeightedDistance( _a, _a, out int distance ), Is.True );
		Assert.That( distance, Is.Zero );
	}

	[Test]
	public void TryGetWeightedDistance_AdjacentNodes_ReturnsWeight() {
		_ = _graph.TryConnectNodes( _a, _b, "edge", 5, out _ );

		Assert.That( _graph.TryGetWeightedDistance( _a, _b, out int distance ), Is.True );
		Assert.That( distance, Is.EqualTo( 5 ) );
	}

	[Test]
	public void TryGetWeightedDistance_CheaperIndirectPath_ReturnsCheaperCost() {
		_ = _graph.TryConnectNodes( _a, _b, "ab", 10, out _ );
		_ = _graph.TryConnectNodes( _a, _c, "ac", 2, out _ );
		_ = _graph.TryConnectNodes( _c, _b, "cb", 3, out _ );

		Assert.That( _graph.TryGetWeightedDistance( _a, _b, out int distance ), Is.True );
		Assert.That( distance, Is.EqualTo( 5 ) );
	}

	[Test]
	public void TryGetWeightedDistance_Disconnected_ReturnsFalse() {
		Assert.That( _graph.TryGetWeightedDistance( _a, _b, out _ ), Is.False );
	}

	[Test]
	public void TryGetWeightedDistance_UnknownNode_ReturnsFalse() {
		INode<string, string> unknown = new Node<string, string>( "unknown" );

		Assert.That( _graph.TryGetWeightedDistance( _a, unknown, out _ ), Is.False );
	}

	[Test]
	public void TryGetWeightedDistance_NullNode_Throws() {
		Assert.That(
			() => _graph.TryGetWeightedDistance( null!, _b, out _ ),
			Throws.ArgumentNullException
		);
	}

	[Test]
	public void TryGetWeightedPath_SameNode_ReturnsNodeOnly() {
		Assert.That( _graph.TryGetWeightedPath( _a, _a, out IReadOnlyList<INode<string, string>> path ), Is.True );
		Assert.That( path, Is.EqualTo( [_a] ) );
	}

	[Test]
	public void TryGetWeightedPath_CheaperIndirectPath_ReturnsCheaperRoute() {
		_ = _graph.TryConnectNodes( _a, _b, "ab", 10, out _ );
		_ = _graph.TryConnectNodes( _a, _c, "ac", 2, out _ );
		_ = _graph.TryConnectNodes( _c, _b, "cb", 3, out _ );

		Assert.That( _graph.TryGetWeightedPath( _a, _b, out IReadOnlyList<INode<string, string>> path ), Is.True );
		Assert.That( path, Is.EqualTo( [_a, _c, _b] ) );
	}

	[Test]
	public void TryGetWeightedPath_Disconnected_ReturnsFalse() {
		Assert.That( _graph.TryGetWeightedPath( _a, _b, out IReadOnlyList<INode<string, string>> path ), Is.False );
		Assert.That( path, Is.Empty );
	}

	[Test]
	public void TryGetWeightedPath_UnknownNode_ReturnsFalse() {
		INode<string, string> unknown = new Node<string, string>( "unknown" );

		Assert.That( _graph.TryGetWeightedPath( _a, unknown, out _ ), Is.False );
	}

	[Test]
	public void TryGetWeightedPath_NullNode_Throws() {
		Assert.That(
			() => _graph.TryGetWeightedPath( null!, _b, out _ ),
			Throws.ArgumentNullException
		);
	}

	[Test]
	public void TryGetDistance_WeightedEdges_ReturnsHopCount() {
		_ = _graph.TryConnectNodes( _a, _c, "ac", 2, out _ );
		_ = _graph.TryConnectNodes( _c, _b, "cb", 3, out _ );

		Assert.That( _graph.TryGetDistance( _a, _b, out int distance ), Is.True );
		Assert.That( distance, Is.EqualTo( 2 ) );
	}

	[Test]
	public void TryVisitWeightedPath_SameNode_VisitsNodeOnly() {
		List<INode<string, string>> visited = [];

		Assert.That( _graph.TryVisitWeightedPath( _a, _a, visited.Add ), Is.True );
		Assert.That( visited, Is.EqualTo( [_a] ) );
	}

	[Test]
	public void TryVisitWeightedPath_CheaperIndirectPath_VisitsCheaperRouteInOrder() {
		_ = _graph.TryConnectNodes( _a, _b, "ab", 10, out _ );
		_ = _graph.TryConnectNodes( _a, _c, "ac", 2, out _ );
		_ = _graph.TryConnectNodes( _c, _b, "cb", 3, out _ );
		List<INode<string, string>> visited = [];

		Assert.That( _graph.TryVisitWeightedPath( _a, _b, visited.Add ), Is.True );
		Assert.That( visited, Is.EqualTo( [_a, _c, _b] ) );
	}

	[Test]
	public void TryVisitWeightedPath_Disconnected_ReturnsFalseAndDoesNotVisit() {
		List<INode<string, string>> visited = [];

		Assert.That( _graph.TryVisitWeightedPath( _a, _b, visited.Add ), Is.False );
		Assert.That( visited, Is.Empty );
	}

	[Test]
	public void TryVisitWeightedPath_UnknownNode_ReturnsFalseAndDoesNotVisit() {
		INode<string, string> unknown = new Node<string, string>( "unknown" );
		List<INode<string, string>> visited = [];

		Assert.That( _graph.TryVisitWeightedPath( _a, unknown, visited.Add ), Is.False );
		Assert.That( visited, Is.Empty );
	}

	[Test]
	public void TryVisitWeightedPath_NullVisitor_Throws() {
		Assert.That(
			() => _graph.TryVisitWeightedPath( _a, _a, null! ),
			Throws.ArgumentNullException
		);
	}

	[Test]
	public void TryVisitWeightedPath_NullNode_Throws() {
		Assert.That(
			() => _graph.TryVisitWeightedPath( null!, _b, _ => { } ),
			Throws.ArgumentNullException
		);
	}
}
