import * as React from 'react';
import {Container} from 'reactstrap';
import NavMenu from './NavMenu';
import Footer from "./Footer";

export default (props: { onSignOut: () => void, children?: React.ReactNode }) => (
  <React.Fragment>
    <NavMenu onSignOut={props.onSignOut}/>
    <Container>
      {props.children}
    </Container>
    <Footer/>
  </React.Fragment>
);
