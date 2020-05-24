import * as React from 'react';
import {useEffect, useMemo} from 'react';
import * as signalR from '@microsoft/signalr';
import {RetryContext} from '@microsoft/signalr';
import {Button} from "reactstrap";

export class RetryPolicy implements signalR.IRetryPolicy {

  constructor(readonly retryDelays: number[]) {
  }

  nextRetryDelayInMilliseconds(retryContext: RetryContext): number | null {
    const index = Math.min(retryContext.previousRetryCount, this.retryDelays.length - 1);
    const retryDelay = this.retryDelays[index];
    return retryDelay;
  }
}

export interface RecordedEvent {
  id: string;
  stream: string;
  eventId: string;
  eventType: string;
  eventNumber: number;
  created: string;
  data: any;
}

const StreamHub = (props: { onEventRecorded: (event: RecordedEvent) => void }) => {

  const connection = useMemo(() => {
    return new signalR.HubConnectionBuilder()
      .withAutomaticReconnect(new RetryPolicy([0, 1_000, 2_000, 5_000, 10_000]))
      .withUrl('/hubs/stream')
      .configureLogging(signalR.LogLevel.Information)
      .build();
  }, []);
  
  const onEventRecorded = props.onEventRecorded;
  
  useEffect(() => {
    connection.start()
      .then(() => console.log('Connected to /hubs/stream'))
      .catch(err => console.error('Failed to connect to /hubs/stream', err));

    connection.on('EventRecorded', onEventRecorded);

    return () => {
      connection.stop()
        .then(() => console.log('Disconnected from /hubs/stream'))
        .catch(err => console.error('Failed to disconnect from /hubs/stream', err));
    }
  }, [connection, onEventRecorded]);

  return <Button onClick={() => connection.send('Test')}>SignalR</Button>;
};

export default StreamHub;
