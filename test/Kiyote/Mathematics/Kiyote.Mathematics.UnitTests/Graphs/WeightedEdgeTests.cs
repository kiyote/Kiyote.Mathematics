using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Mathematics.Graphs.UnitTests;

[TestFixture]
[ExcludeFromCodeCoverage]
internal sealed class WeightedEdgeTests {

	private INode<string, string> _a;
	private INode<string, string> _b;
	private IWeightedEdge<string, string> _edge;

	[SetUp]
	public void SetUp() {
		_a = new Node<string, string>( "a" );
		_b = new Node<string, string>( "b" );
		_edge = new WeightedEdge<string, string>( "edge", _a, _b, 7 );
	}

	[Test]
	public void Context_ValueSupplied_ReturnsValue() {
		Assert.That( _edge.Context, Is.EqualTo( "edge" ) );
	}

	[Test]
	public void A_ValueSupplied_ReturnsNode() {
		Assert.That( _edge.A, Is.SameAs( _a ) );
	}

	[Test]
	public void B_ValueSupplied_ReturnsNode() {
		Assert.That( _edge.B, Is.SameAs( _b ) );
	}

	[Test]
	public void Weight_ValueSupplied_ReturnsWeight() {
		Assert.That( _edge.Weight, Is.EqualTo( 7 ) );
	}
}
