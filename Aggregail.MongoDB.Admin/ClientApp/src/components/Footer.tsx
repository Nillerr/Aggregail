import React from "react";
import {Container} from "reactstrap";
import {Link} from "react-router-dom";

const Footer = () => (
  <Container fluid={true} tag="footer" className="text-center mb-5 mt-5">
    Aggregail MongoDB Admin 1.0.0.0-alpha.1 · <Link to="https://github.com/nillerr/Aggregail">Documentation</Link>
  </Container>
);

export default Footer;