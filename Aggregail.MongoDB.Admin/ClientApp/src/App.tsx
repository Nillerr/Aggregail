import * as React from 'react';
import {Redirect, Route, Switch} from 'react-router';
import Layout from './components/Layout';
import StreamBrowserPage from './components/StreamBrowserPage';
import StreamPage from "./components/StreamPage";
import EventPage from "./components/EventPage";
import DashboardPage from "./components/DashboardPage";

import './custom.css'
import UsersPage from "./components/UsersPage";
import NewUserPage from "./components/NewUserPage";
import LoginPage from "./components/LoginPage";
import {useState} from "react";
import {User} from "./model";

const Session = (props: { server: string }) => (
  <Layout>
    <Switch>
      <Route exact path='/' render={() => <Redirect to={'/streams'}/>}/>
      <Route exact path='/dashboard' component={DashboardPage}/>
      <Route exact path='/users' render={() =>
        <UsersPage key="/users"/>
      }/>
      <Route exact path='/users/new' component={NewUserPage}/>
      <Route exact path='/streams' render={props =>
        <StreamBrowserPage key="/streams" onStreamBrowse={stream => props.history.push(`/streams/${stream}`)}/>
      }/>
      <Route exact path='/streams/:name' render={props =>
        <StreamPage key={`/streams/${props.match.params.name}`} name={props.match.params.name}/>
      }/>
      <Route exact path='/streams/:name/:eventNumber' render={props =>
        <EventPage
          key={`/streams/${props.match.params.eventNumber}/${props.match.params.name}`}
          stream={props.match.params.name}
          eventNumber={parseInt(props.match.params.eventNumber)}
        />
      }/>
      <Route render={() => <Redirect to="/"/>}/>
    </Switch>
  </Layout>
);

const Login = (props: { onSignIn: (server: string) => void }) => (
  <Switch>
    <Route exact path="/" render={() => <LoginPage onSignIn={props.onSignIn}/>}/>
    <Route render={() => <Redirect to="/"/>}/>
  </Switch>
);

const App = () => {
  const [server, setServer] = useState<string>();
  const [user, setUser] = useState<User>();
  
  return server ? <Session server={server}/> : <Login onSignIn={s => setServer(s)}/>;
};

export default App;
