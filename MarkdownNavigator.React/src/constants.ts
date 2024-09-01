import { ResultTextDTO } from "./types";

export const Domain = process.env.REACT_APP_DOMAIN || "http://localhost:3000/";

export const LocalStorageParams = {
  EDITORSTATE: 'EditorState'
};

export const DefaultResultMarkdownDTO : ResultTextDTO = { 
  isError: false, 
  errorMessage: '', 
  text: ''
}