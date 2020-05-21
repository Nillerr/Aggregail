import * as React from 'react';
import {Redirect, Route, Switch} from 'react-router';
import Layout from './components/Layout';
import StreamsPage from './components/StreamsPage';
import StreamPage from "./components/StreamPage";
import EventPage from "./components/EventPage";
import DashboardPage from "./components/DashboardPage";

import './custom.css'

export default () => (
  <Layout>
    <Switch>
      <Route exact path='/' render={() => <Redirect to={'/dashboard'}/>}/>
      <Route exact path='/dashboard' component={DashboardPage}/>
      <Route exact path='/streams' component={StreamsPage}/>
      <Route exact path='/streams/:name' render={props =>
        <StreamPage key={props.match.params.name} name={props.match.params.name}/>
      }/>
      <Route exact path='/streams/:name/:eventNumber' render={props =>
        <EventPage
          key={`${props.match.params.eventNumber}@${props.match.params.name}`}
          stream={props.match.params.name}
          eventNumber={parseInt(props.match.params.eventNumber)}
        />
      }/>
    </Switch>
  </Layout>
);
