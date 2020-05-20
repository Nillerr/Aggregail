import * as React from 'react';
import {Route} from 'react-router';
import Layout from './components/Layout';
import Dashboard from './components/Dashboard';
import Stream from "./components/Stream";
import Event from "./components/Event";
import Home from './components/Home';
import Counter from './components/Counter';
import FetchData from './components/FetchData';

import './custom.css'

export default () => (
  <Layout>
    <Route exact path='/' component={Dashboard}/>
    <Route exact path='/home' component={Home}/>
    <Route exact path='/streams/:name' component={Stream}/>
    <Route exact path='/streams/:name/:eventNumber' render={props => 
      <Event stream={props.match.params.name} eventNumber={parseInt(props.match.params.eventNumber)}/>
    }/>
    <Route exact path='/counter' component={Counter}/>
    <Route exact path='/fetch-data/:startDateIndex?' component={FetchData}/>
  </Layout>
);
