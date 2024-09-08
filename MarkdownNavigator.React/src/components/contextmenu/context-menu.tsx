import React, { useEffect, useRef } from 'react';
import { ContextMenuActions } from '../../constants';
import "./context-menu.css";

interface IProps {
  x: number;
  y: number;
  isVisible: boolean;
  onClose: () => void;
  onAction: (action: string) => void;
}

const ContextMenu: React.FunctionComponent<IProps> = ({ x, y, isVisible, onClose, onAction }) => {
  const menuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
        onClose();
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [onClose]);

  if (!isVisible) {
    return null;
  }

  return (
    <div
      ref={menuRef}
      className="context-menu"
      style={{ top: `${y}px`, left: `${x}px`}}
    >
      <ul>
        <li onClick={() => onAction(ContextMenuActions.SAVE)}>Save</li>
        <li onClick={() => onAction(ContextMenuActions.PASTEIMAGE)}>Paste image</li>
      </ul>
    </div>
  );
};

export default ContextMenu;
