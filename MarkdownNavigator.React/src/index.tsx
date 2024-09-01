import React from 'react';
import ReactDOM from 'react-dom/client';
import {
  createBrowserRouter,
  RouterProvider
} from "react-router-dom";
import EditPage from './components/pages/edit-page';
import "./assets/css/style.css";
import "./assets/css/style-react.css";



const router = createBrowserRouter([
  {
    path: "*",
    element: <EditPage />,
  },
]);

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

root.render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>
)
