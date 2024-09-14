import React, { useEffect } from "react";
import { IEditorState, ResultTextDTO } from "../../types";
import {
  useGetFetch,
  useLocalStorage,
  usePostFetch,
  useTextDebounce,
} from "../../customHooks";
import {
  DefaultResultMarkdownDTO,
  EditorMode,
  LocalStorageParams,
} from "../../constants";
import HtmlView from "../htmlview/html-view";
import Header from "../header/header";
import Textarea from "../textarea/textarea";

const useGetHtmlResult = (
  editorMode: string,
  previewText: string
): ResultTextDTO => {
  const isInvalidRequest =
    editorMode !== EditorMode.PREVIEW || previewText.length === 0;

  return usePostFetch<ResultTextDTO>(
    "api/html",
    DefaultResultMarkdownDTO,
    { Text: previewText },
    previewText,
    isInvalidRequest
  );
};

const useGetMarkdownResult = (): ResultTextDTO => {
  const path = window.location.pathname.substring(1);
  const isInvalidRequest = !(path && path.endsWith(".md"));

  return useGetFetch<ResultTextDTO>(
    `api/markdown?path=${path}`,
    DefaultResultMarkdownDTO,
    path,
    isInvalidRequest
  );
};

const useGetHelpResult = (): ResultTextDTO => {
  return useGetFetch<ResultTextDTO>("api/html", DefaultResultMarkdownDTO, "");
};

const EditPage: React.FunctionComponent = () => {
  const [editorState, setEditorState] = useLocalStorage<IEditorState>(
    LocalStorageParams.EDITORSTATE,
    { editorMode: EditorMode.DEFAULT }
  );

  const markdownResult = useGetMarkdownResult();
  const markdownResultText = markdownResult.text || "";

  const [previewText, setPreviewText] = useTextDebounce(
    markdownResultText,
    1000
  );

  const previewHtmlResult = useGetHtmlResult(
    editorState.editorMode,
    previewText
  );
  const helpHtmlResult = useGetHelpResult();

  useEffect(() => {
    setPreviewText(markdownResultText);
  }, [markdownResultText, editorState.editorMode]);

  return (
    <>
      <Header editorState={editorState} setEditorState={setEditorState} />
      <main className="main flex">
        {markdownResult.isError ? (
          <div className="container-text">
            <h1>An error has occurred!</h1>
            <h3>Error message: {markdownResult.errorMessage}</h3>
          </div>
        ) : (
          <div className="flex">
            <Textarea
              editorMode={editorState.editorMode}
              markdownResultText={markdownResultText}
              setPreviewText={setPreviewText}
            />
            {editorState.editorMode !== EditorMode.DEFAULT && (
              <HtmlView
                html={
                  editorState.editorMode === EditorMode.PREVIEW
                    ? previewHtmlResult.text!
                    : helpHtmlResult.text!
                }
              />
            )}
          </div>
        )}
      </main>
    </>
  );
};

export default EditPage;
