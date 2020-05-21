import * as React from 'react';
import {useEffect, useState} from 'react';
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
import {User} from "./model";
import Axios from "axios";
import querystring from 'querystring';

interface RoutedStreamPageProps {
  name: string;
  from: number;
  limit: number;
  navigate: (path: string) => void;
}

const RoutedStreamPage = (props: RoutedStreamPageProps) => {
  return <StreamPage
    key={`/streams/${props.name}`}
    from={props.from}
    name={props.name}
    limit={props.limit}
    onReset={() => {
      props.navigate(`/streams/${props.name}`);
    }}
    onFirst={() => {
      props.navigate(`/streams/${props.name}/0/forward/${props.limit}`);
    }}
    onPrevious={() => {
      props.navigate(`/streams/${props.name}/${Math.max(0, props.from - props.limit)}/forward/${props.limit}`);
    }}
    onNext={() => {
      props.navigate(`/streams/${props.name}/${props.from + props.limit}/forward/${props.limit}`);
    }}
  />;
}

const intParam = (value: string | undefined, defaultValue: number) => {
  return value === null || value === undefined ? defaultValue : parseInt(value);
};

const minMax = (value: number, options: { min: number, max: number }) => {
  return Math.max(options.min, Math.min(options.max, value));
}

const Session = (props: { onSignOut: () => void }) => {
  return (
    <Layout onSignOut={props.onSignOut}>
      <Switch>
        <Route exact path='/' render={() => <Redirect to={'/streams'}/>}/>
        <Route exact path='/dashboard' component={DashboardPage}/>
        <Route exact path='/users' render={() =>
          <UsersPage key="/users"/>
        }/>
        <Route exact path='/users/new' component={NewUserPage}/>
        <Route exact path='/streams' render={route =>
          <StreamBrowserPage key="/streams" onStreamBrowse={stream => route.history.push(`/streams/${stream}`)}/>
        }/>
        <Route exact path='/streams/:name' render={props =>
          <RoutedStreamPage
            name={props.match.params.name}
            from={0}
            limit={20}
            navigate={props.history.push}
          />
        }/>
        <Route exact path='/streams/:name/:from/forward/:limit' render={props =>
          <RoutedStreamPage
            name={props.match.params.name}
            from={Math.max(0, intParam(props.match.params.from, 0))}
            limit={minMax(intParam(props.match.params.limit, 20), { min: 0, max: 20 })}
            navigate={props.history.push}
          />
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
};

const returnUrl = () => {
  const path = `${window.location.pathname}${window.location.search}${window.location.hash}`;
  return encodeURIComponent(path);
};

const Login = (props: { onSignIn: () => void }) => {
  return (
    <Switch>
      <Route exact path="/" render={route => {
        const query: any = querystring.parse(route.location.search.substring(1));
        return <LoginPage onSignIn={() => {
          props.onSignIn();
          route.history.push(query.returnTo || '/');
        }}/>;
      }
      }/>
      <Route render={() => <Redirect to={{
        pathname: '/',
        search: `returnTo=${returnUrl()}`
      }}/>}/>
    </Switch>
  );
};

const App = () => {

  const [isSignedIn, setIsSignedIn] = useState<boolean>();

  useEffect(() => {
    if (isSignedIn === false) {
      return;
    } else if (isSignedIn === undefined) {
      const cts = Axios.CancelToken.source();

      Axios
        .get<User>('/api/userinfo', {cancelToken: cts.token, withCredentials: true})
        .then(response => setIsSignedIn(true))
        .catch(() => {
          setIsSignedIn(false);
          Axios.post('/api/auth/logout', {cancelToken: cts.token})
            .then(() => console.log('Signed out'))
            .catch(reason => console.error(reason));
        });

      return () => {
        cts.cancel();
      };
    }
  }, [isSignedIn]);

  switch (isSignedIn) {
    case undefined:
      return null;
    case true:
      return <Session onSignOut={() => setIsSignedIn(false)}/>;
    case false:
      return <Login onSignIn={() => setIsSignedIn(true)}/>;
  }
};

export default App;
