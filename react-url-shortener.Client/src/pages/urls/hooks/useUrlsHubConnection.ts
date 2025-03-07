import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import Environment from "@src/environment/environment";
import { useState, useEffect } from "react";

const useUrlsHubConnection = () => {
  const [connection, setConnection] = useState<HubConnection>();

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
      .withUrl(`${Environment.rootApiUrl}/urlsHub`)
      .withStatefulReconnect()
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);

    return () => {
      newConnection
        .stop()
        .catch((e) => console.error("[SignalR]: Error stopping connection:", e.message));
    };
  }, []);

  useEffect(() => {
    if (!connection) return;

    const startConnection = async () => {
      try {
        await connection.start();
        console.log("[SignalR]: Connection started");
      } catch (e: any) {
        console.error("[SignalR]: Error starting connection:", e.message);
      }
    };

    startConnection();

    connection.onreconnected(() => {
      console.log("[SignalR]: Connection reestablished");
    });

    return () => {
      connection
        .stop()
        .catch((e) => console.error("[SignalR]: Error stopping connection", e.message));
    };
  }, [connection]);

  return connection;
};

export default useUrlsHubConnection;
