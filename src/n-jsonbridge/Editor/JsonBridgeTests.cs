#if N_JSONBRIDGE_TESTS
using N.Package.JsonBridge;
using N.Package.JsonBridge.Internal;
using NUnit.Framework;
using UnityEditor;

public class JsonBridgeTests : N.Tests.Test
{
    [Test]
    public void TestCreateBridge()
    {
        new JsonBridge(new MockTransport());
    }

    [Test]
    public void TestOpenConnection()
    {
        var transport = new MockTransport();
        var bridge = new JsonBridge(transport);

        var connected = false;
        bridge.Connect("127.0.0.1", 1000);
        bridge.AddEventHandler<ConnectionOpenedEvent>((ep) => { connected = true; });

        transport.TriggerConnection();
        bridge.Update();

        Assert(connected);
    }

    [Test]
    public void TestCloseConnection()
    {
        var transport = new MockTransport();
        var bridge = new JsonBridge(transport);

        var connected = false;
        bridge.Connect("127.0.0.1", 1000);
        bridge.AddEventHandler<ConnectionOpenedEvent>((ep) => { connected = true; });
        bridge.AddEventHandler<ConnectionClosedEvent>((ep) => { connected = false; });

        transport.TriggerConnection();
        transport.TriggerDisconnect();
        bridge.Update();

        Assert(!connected);
    }

    [Test]
    public void TestReceiveData()
    {
        var transport = new MockTransport();
        var bridge = new JsonBridge(transport);

        var message = new TestMessage() { Message = "Hello World" };

        var gotMessage = false;
        bridge.Connect("127.0.0.1", 1000);
        bridge.AddEventHandler<ConnectionOpenedEvent>((ep) => { bridge.Write(message); });
        bridge.AddEventHandler<DataReceivedEvent>((ep) =>
        {
            gotMessage = true;

            var msg = ep.Read<TestMessage>();
            Assert(msg != null);
            Assert(msg.Message == "Hello World");
        });

        transport.TriggerConnection();
        transport.TriggerData(message);
        bridge.Update();

        Assert(gotMessage);
    }
}

public class TestMessage
{
    public string Message;
}

#endif