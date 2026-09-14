using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Mathematics.Graphs.UnitTests;

[TestFixture]
[ExcludeFromCodeCoverage]
internal sealed class NodeTests {

	private INode<string, string> _node;

	[SetUp]
	public void SetUp() {
		_node = new Node<string, string>( "node" );
	}

	[Test]
	public void Context_ValueSupplied_ReturnsValue() {
		Assert.That( _node.Context, Is.EqualTo( "node" ) );
	}

	[Test]
	public void Context_NullSupplied_ReturnsNull() {
		INode<string, string> node = new Node<string, string>( null );

		Assert.That( node.Context, Is.Null );
	}
}
