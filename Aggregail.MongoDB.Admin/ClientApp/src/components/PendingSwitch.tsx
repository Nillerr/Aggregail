import {SwitchProps, __RouterContext as RouterContext, matchPath, RouteChildrenProps} from "react-router";
import React, {useRef, useState} from "react";
import Axios, {CancelToken, CancelTokenSource} from "axios";
import * as H from 'history';

export type Resolver = (props: RouteChildrenProps, cancelToken: CancelToken) => Promise<any>;

export const PendingSwitch = (props: SwitchProps) => {
  
  const previousElementRef = useRef<React.ReactNode>();
  const previousLocationRef = useRef<H.Location>()
  
  const [isResolving, setIsResolving] = useState(false);
  
  const currentCancelTokenSourceRef = useRef<CancelTokenSource>();
  
  return (
    <RouterContext.Consumer>
      {context => {

        const location = this.props.location || context.location;

        if (isResolving) {
          if (previousLocationRef.current === location) {
            const previousElement = previousElementRef.current;
            return previousElement
              ? previousElement
              : (<div>Loading...</div>);
          } else {
            currentCancelTokenSourceRef.current.cancel();
            currentCancelTokenSourceRef.current = undefined;
          }
        }
        
        previousLocationRef.current = location;

        let element, match;

        // We use React.Children.forEach instead of React.Children.toArray().find()
        // here because toArray adds keys to all child elements and we do not want
        // to trigger an unmount/remount for two <Route>s that render the same
        // component at different URLs.
        React.Children.forEach(this.props.children, child => {
          if (match == null && React.isValidElement(child)) {
            element = child;

            const path = child.props.path || child.props.from;

            match = path
              ? matchPath(location.pathname, { ...child.props, path })
              : context.match;
          }
        });

        if (match) {
          const resolver = element.props.resolve;
          if (resolver) {
            const cts = Axios.CancelToken.source();
            currentCancelTokenSourceRef.current = cts;
            
            resolver(props, cts);
            setIsResolving(true);
            
            return previousElementRef.current;
          } else {
            const clone = React.cloneElement(element, {location, computedMatch: match});
            previousElementRef.current = clone;
            return clone;
          }
        } else {
          return null;
        }
      }}
    </RouterContext.Consumer>
  );
}

export default PendingSwitch;