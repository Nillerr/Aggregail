import React, {useEffect, useState} from "react";
import {Link} from "react-router-dom";
import Axios from "axios";

const StreamRow = (props: { name: string }) => (
  <tr>
    <td><Link to={`/streams/${props.name}`}>{props.name}</Link></td>
  </tr>
);

const StreamsTable = (props: { legend: string, streams: RecentStream[] | null }) => (
  <table className="table table-bordered">
    <thead className="thead-dark">
    <tr>
      <th scope="col">{props.legend}</th>
    </tr>
    </thead>
    <tbody>
    {props.streams === null
      ? <tr><td>Loading...</td></tr>
      : props.streams.map(stream => <StreamRow key={stream.name} name={stream.name}/>)
    }
    </tbody>
  </table>
);

interface RecentStream {
  name: string;
}

interface RecentStreams {
  recentlyCreatedStreams: RecentStream[];
  recentlyChangedStreams: RecentStream[];
}

type RecentStreamsState = 
  | { kind: 'Loading' } 
  | { kind: 'Loaded' } & RecentStreams;

const StreamsPage = () => {
  
  const [recentStreamsState, setRecentStreamsState] = useState<RecentStreamsState>({ kind: 'Loading' });

  useEffect(() => {
    const cts = Axios.CancelToken.source();
    
    Axios
      .get<RecentStreams>('/api/streams', { cancelToken: cts.token })
      .then(response => setRecentStreamsState({ kind: 'Loaded', ...response.data }))
      .catch(err => {
        if (!Axios.isCancel(err)) {
          console.error(err);
        }
      });
    
    return () => cts.cancel();
  }, [])
  
  return (
    <div className="row">
      <div className="col-12 col-md-6">
        <StreamsTable
          legend="Recently Created Streams"
          streams={recentStreamsState.kind === 'Loaded' ? recentStreamsState.recentlyCreatedStreams : null}
        />
      </div>
      <div className="col-12 col-md-6">
        <StreamsTable
          legend="Recently Changed Streams"
          streams={recentStreamsState.kind === 'Loaded' ? recentStreamsState.recentlyChangedStreams : null}
        />
      </div>
    </div>
  );

}

export default StreamsPage;