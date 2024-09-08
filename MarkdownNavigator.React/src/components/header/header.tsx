import React from "react";
import { HelpIcon, PreviewIcon } from "../icons/icons";
import { EditorMode } from "../../constants";
import { IEditorState } from "../../types";
import "./header.css";

interface IProps {
  editorState: IEditorState;
  setEditorState: React.Dispatch<React.SetStateAction<IEditorState>>;
}

const Header: React.FunctionComponent<IProps> = ({
  editorState,
  setEditorState,
}) => {
  const changeEditorMode = (editorMode: string): void => {
    const newMode = editorState.editorMode === editorMode 
      ? EditorMode.DEFAULT 
      : editorMode;

    const newEditorState = { ...editorState, editorMode: newMode };
    setEditorState(newEditorState);
  };

  const handlePreviewClick = (): void => {
    changeEditorMode(EditorMode.PREVIEW);
  };

  const handleHelpClick = (): void => {
    changeEditorMode(EditorMode.HELP);
  };

  const getHeaderIconClass = (editorMode: string): string => {
    return editorState.editorMode === editorMode
      ? "button-icon button-icon-active button-icon--help"
      : "button-icon button-icon--help";
  };

  return (
    <header className="header flex flex-center">
      <div className="flex flex-center">
        <a
          className="header-text header-link"
          href={`${process.env.PUBLIC_URL}/index.md`}
        >
          Editor.md
        </a>
      </div>
      <div
        className={getHeaderIconClass(EditorMode.PREVIEW)}
        title="Preview mode"
        onClick={handlePreviewClick}
      >
        {PreviewIcon}
      </div>
      <div
        className={getHeaderIconClass(EditorMode.HELP)}
        title="Help mode"
        onClick={handleHelpClick}
      >
        {HelpIcon}
      </div>
    </header>
  );
};

export default Header;
