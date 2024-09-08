import React, { useEffect, useRef } from "react";
import "./html-view.css";

interface IProps {
  html: string
}

const HtmlView: React.FunctionComponent<IProps> = ({html}) => {
  const htmlRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (htmlRef.current) {
      // @ts-ignore
      Prism.highlightAllUnder(htmlRef.current);
      // @ts-ignore
      applyCodeCopy();
    }
  }, [html]);

  return (
    <div
      ref={htmlRef}
      className="editor-preview blog"
      dangerouslySetInnerHTML={{__html:html}}
    />
  );
};

export default HtmlView;
