import { IContextMenuState, ResultTextDTO } from "./types";

export const Domain = process.env.REACT_APP_DOMAIN || "http://localhost:3000/";

export const LocalStorageParams = {
  EDITORSTATE: "EditorState"
};

export const EditorMode = {
  DEFAULT: "Default",
  PREVIEW: "Preview",
  HELP: "Help"
};

export const ContextMenuActions = {
  SAVE: "Save",
  PASTEIMAGE: "PasteImage"
}

export const DefaultContextMenuState : IContextMenuState = { 
  isVisible: false,
  x: 0,
  y: 0,
}

export const DefaultResultMarkdownDTO : ResultTextDTO = { 
  isError: false, 
  errorMessage: "", 
  text: ""
}