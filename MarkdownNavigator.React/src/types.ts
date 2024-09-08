export interface ResultTextDTO {
  isError: boolean;
  errorMessage: string | null;
  text: string | null;
}

export interface IEditorState {
  editorMode: string;
}

export interface IContextMenuState {
  isVisible: boolean;
  x: number;
  y: number;
}