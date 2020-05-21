import * as React from 'react';
import {Container} from 'reactstrap';
import NavMenu from './NavMenu';
import Footer from "./Footer";

export default (props: { children?: React.ReactNode }) => (
  <React.Fragment>
    <NavMenu/>
    <Container>
      {props.children}
    </Container>
    <Footer/>
  </React.Fragment>
);
