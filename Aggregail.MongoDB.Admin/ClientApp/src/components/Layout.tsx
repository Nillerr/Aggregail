import * as React from 'react';
import {Container} from 'reactstrap';
import NavMenu from './NavMenu';

export default (props: { children?: React.ReactNode }) => (
  <React.Fragment>
    <NavMenu/>
    <Container>
      {props.children}
    </Container>
    <Container fluid={true} tag="footer" className="text-center mb-5 mt-5">
      Aggregail MongoDB Admin 1.0.0.0-alpha.1
    </Container>
  </React.Fragment>
);
