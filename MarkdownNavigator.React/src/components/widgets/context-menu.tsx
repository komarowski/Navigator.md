import React, { useEffect, useRef } from 'react';

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
      style={{
        position: 'absolute',
        top: `${y}px`,
        left: `${x}px`,
        backgroundColor: '#fff',
        border: '1px solid #ccc',
        boxShadow: '0px 0px 5px rgba(0, 0, 0, 0.2)',
        zIndex: 100,
      }}
    >
      <ul style={{ listStyle: 'none', padding: '5px 10px', margin: 0 }}>
        <li onClick={() => onAction('Insert Details block')}>Insert Details block</li>
        <li onClick={() => onAction('Insert Code block')}>Insert Code block</li>
      </ul>
    </div>
  );
};

export default ContextMenu;
