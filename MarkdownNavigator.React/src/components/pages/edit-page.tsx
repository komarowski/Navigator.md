import React, { useEffect, useRef, useState } from "react";
import { IEditorState, ResultTextDTO } from "../../types";
import { useGetFetch, useLocalStorage, usePostFetch, useTextDebounce } from "../../customHooks";
import { DefaultResultMarkdownDTO, LocalStorageParams } from "../../constants";
import { PreviewIcon, SaveIcon } from "../icons/icons";
import { postData } from "../../utils";
import ContextMenu from "../widgets/context-menu";


const useGetHtmlResult = (previewMode: boolean, previewText: string): ResultTextDTO => {
  const isInvalidRequest = !previewMode || previewText.length === 0;
  
  return usePostFetch<ResultTextDTO>('api/html', DefaultResultMarkdownDTO, { Text: previewText }, previewText, isInvalidRequest);
};

const useGetMarkdownResult = (): ResultTextDTO => {
  const path = window.location.pathname.substring(1);
  const isInvalidRequest = !(path && path.endsWith(".md")) 

  return useGetFetch<ResultTextDTO>(`api/markdown?path=${path}`, DefaultResultMarkdownDTO, path, isInvalidRequest);
};

const EditPage: React.FunctionComponent = () => {
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  
  const [contextMenu, setContextMenu] = useState({
    isVisible: false,
    x: 0,
    y: 0,
  });

  const markdownResult = useGetMarkdownResult();
  const markdownResultText = markdownResult.text || "";

  const [markdownText, setMarkdownText] = useState(markdownResultText);
  const [previewText, setPreviewText] = useTextDebounce(markdownResultText, 1000);
  const [editorState, setEditorState] = useLocalStorage<IEditorState>(LocalStorageParams.EDITORSTATE, { previewMode: true } );

  const htmlResult = useGetHtmlResult(editorState.previewMode, previewText);

  useEffect(() => {
    setMarkdownText(markdownResultText);
    setPreviewText(markdownResultText);
  }, [markdownResultText]);

  const handlePreviewClick = (): void => {
    const newEditorState = { ...editorState, previewMode: !editorState.previewMode };
    setEditorState(newEditorState);
    setPreviewText(markdownResultText);
  }

  const handleSaveClick = async (): Promise<void> => {
    if (window.confirm("Confirm file saving") === true) {
      const markdownEditDTO = {
        Path: window.location.pathname.substring(1),
        Text: markdownText
      }
      const result = await postData("api/markdown", markdownEditDTO);
      if (result.isError){
        alert(result.errorMessage);
      }
    }
  }

  const handleContextMenu = (event: React.MouseEvent) => {
    event.preventDefault();
    setContextMenu({
      isVisible: true,
      x: event.clientX,
      y: event.clientY - 20,
    });
  };

  const handleCloseMenu = () => {
    setContextMenu({
      isVisible: false,
      x: 0,
      y: 0,
    });
  };

  const handleAction = (action: string) => {
    if (!textareaRef.current) return;
    const position = textareaRef.current.selectionStart;

    let newText = '';
    switch (action) {
      case 'Insert Details block':
        newText = `<details>\n<summary></summary>\n\n</details>`;
        break;
      case 'Insert Code block':
        newText = "```csharp\n\n```";
        break;
      default:
        break;
    }

    const newMarkdownText = markdownText.substring(0, position) + newText + markdownText.substring(position);
    setMarkdownText(newMarkdownText);
    handleCloseMenu();
  };

  return (
    <>
      <header className="header flex flex-center">
        <div className="flex flex-center">
          <a className="header-text header-link" href={`${process.env.PUBLIC_URL}/index.md`}>
            Editor.md
          </a>
        </div>
        <div className="button-icon button-icon--help" title="Preview mode" onClick={handlePreviewClick}>
          {PreviewIcon}
        </div>
        <div className="header-divider " />
        <div className="button-icon button-icon--help" title="Save" onClick={handleSaveClick}>
          {SaveIcon}
        </div>
      </header>
      <main className="main flex">
        {
          markdownResult.isError 
            ? <div className="container-text">
                <h1>An error has occurred!</h1>
                <h3>Error message: {markdownResult.errorMessage}</h3>
              </div> 
            : <div className="editor-container">
                <textarea
                  ref={textareaRef}
                  className={`editor-textarea ${editorState.previewMode ? "editor-textarea--preview" : "editor-textarea--full"}`}
                  value={markdownText!}
                  onChange={(e) => {setMarkdownText(e.target.value); setPreviewText(e.target.value)}}
                  onContextMenu={handleContextMenu}
                />
                <ContextMenu
                  x={contextMenu.x}
                  y={contextMenu.y}
                  isVisible={contextMenu.isVisible}
                  onClose={handleCloseMenu}
                  onAction={handleAction}
                />
                {editorState.previewMode && (
                  <div
                    className="editor-preview blog"
                    dangerouslySetInnerHTML={{ __html: htmlResult.text! }}
                  />
                )}
              </div>
        }
      </main>
    </>
  );
};

export default EditPage;
