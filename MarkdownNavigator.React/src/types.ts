export interface ResultTextDTO {
  isError: boolean;
  errorMessage: string | null;
  text: string | null;
}

export interface IEditorState {
  previewMode: boolean;
}