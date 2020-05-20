import React from "react";
import {Link} from "react-router-dom";

const recentlyCreatedStreams = [
  "Case-129998d1-023b-4151-ac93-59ea60c6ef0f",
  "Case-a2ffa2af-8f8a-4eb3-850e-7b32349978d4",
];

const recentlyChangedStreams = [
  "Case-c5de1b9d-9b43-4954-9f9f-ff3cdcf4bce5"
];

const StreamRow = (props: { name: string }) => (
  <tr>
    <td><Link to={`/streams/${props.name}`}>{props.name}</Link></td>
  </tr>
);

const StreamsTable = (props: { legend: string, streams: string[] }) => (
  <table className="table table-bordered">
    <thead className="thead-dark">
    <tr>
      <th scope="col">{props.legend}</th>
    </tr>
    </thead>
    <tbody>
    {props.streams.map(stream => <StreamRow key={stream} name={stream}/>)}
    </tbody>
  </table>
);

const Dashboard = () => {

  return (
    <div className="row">
      <div className="col-12 col-md-6">
        <StreamsTable legend="Recently Created Streams" streams={recentlyCreatedStreams}/>
      </div>
      <div className="col-12 col-md-6">
        <StreamsTable legend="Recently Changed Streams" streams={recentlyChangedStreams}/>
      </div>
    </div>
  );

}

export default Dashboard;