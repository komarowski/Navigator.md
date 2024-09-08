import React, { useEffect, useRef, useState } from "react";
import ContextMenu from "../contextmenu/context-menu";
import { ContextMenuActions, DefaultContextMenuState, EditorMode } from "../../constants";
import { postData, postImage } from "../../utils";
import { IContextMenuState, ResultTextDTO } from "../../types";
import "./textarea.css";

interface IProps {
  editorMode: string,
  markdownResultText: string,
  setPreviewText: React.Dispatch<React.SetStateAction<string>>
}

const Textarea: React.FunctionComponent<IProps> = ({editorMode, markdownResultText, setPreviewText}) => {
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  const [markdownText, setMarkdownText] = useState(markdownResultText);

  const [contextMenu, setContextMenu] = useState<IContextMenuState>(DefaultContextMenuState);

  useEffect(() => {
    setMarkdownText(markdownResultText);
  }, [markdownResultText]);

  const saveMarkdown = async (): Promise<void> => {
    const markdownEditDTO = {
      Path: window.location.pathname.substring(1),
      Text: markdownText,
    };
    const result = await postData("api/markdown", markdownEditDTO);
    if (result.isError) {
      alert(result.errorMessage);
    }
  };

  const uploadImage = async (file: File): Promise<ResultTextDTO> => {
    const formData = new FormData();
    formData.append('image', file);
    formData.append('markdownPath', window.location.pathname.substring(1));
    return await postImage("api/image", formData);
  };

  const saveImage = async () => {
    try {
      const clipboardItems = await navigator.clipboard.read();
      for (const item of clipboardItems) {
        if (item.types.includes('image/png') || item.types.includes('image/jpeg')) {
          const blob = await item.getType(item.types[0]);
          const imageName = `image${Date.now()}.jpg`;
          const file = new File([blob], imageName, { type: blob.type });

          const result = await uploadImage(file);

          if (result.isError){
            alert(result.errorMessage);
            break;
          }

          if (textareaRef.current){
            const position = textareaRef.current.selectionStart;
            const newMarkdownText =
              markdownText.substring(0, position) +
              `\n![Clipboard ${imageName}](${imageName})\n` +
              markdownText.substring(position);
            setMarkdownText(newMarkdownText);
          }
          break;
        }
      }
    } catch (error) {
      console.error('Error reading clipboard:', error);
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
    setContextMenu(DefaultContextMenuState);
  };

  const handleAction = (action: string) => {
    switch (action) {
      case ContextMenuActions.SAVE:
        saveMarkdown();
        break;
      case ContextMenuActions.PASTEIMAGE:
        saveImage();
        break;
      default:
        break;
    }

    handleCloseMenu();
  };

  const getTextareaClass = (): string => {
    return editorMode === EditorMode.DEFAULT
      ? "editor-textarea editor-textarea--full"
      : "editor-textarea editor-textarea--preview";
  }; 

  return (
    <>
      <textarea
        ref={textareaRef}
        className={getTextareaClass()}
        value={markdownText!}
        onChange={(e) => {
          setMarkdownText(e.target.value);
          setPreviewText(e.target.value);
        }}
        onContextMenu={handleContextMenu}
      />
      <ContextMenu
        x={contextMenu.x}
        y={contextMenu.y}
        isVisible={contextMenu.isVisible}
        onClose={handleCloseMenu}
        onAction={handleAction}
      />
    </>
  );
};

export default Textarea;
