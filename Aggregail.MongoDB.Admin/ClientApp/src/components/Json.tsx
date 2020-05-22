import React from "react";
import {Light as SyntaxHighlighter} from "react-syntax-highlighter";
import json from 'react-syntax-highlighter/dist/esm/languages/hljs/json';
import lightStyle from 'react-syntax-highlighter/dist/esm/styles/hljs/idea';
import darkStyle from 'react-syntax-highlighter/dist/esm/styles/hljs/vs2015';

SyntaxHighlighter.registerLanguage('json', json);

const Json = (props: React.PropsWithChildren<{}>) => {
  const themeStyle = localStorage.getItem('theme-style') ?? 'light';
  const style = themeStyle === 'light' ? lightStyle : darkStyle;

  return (
    <SyntaxHighlighter language="json" style={style}>
      {props.children}
    </SyntaxHighlighter>
  );
}


export default Json;