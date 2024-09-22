import React, {useEffect} from 'react';
import { Route, Routes, useNavigate, useLocation  } from 'react-router-dom';
import AppRoutes from './AppRoutes';
import { Layout } from './components/Layout';
import './custom.css';

const App = () => {
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    const token = window.localStorage.getItem("token");

    if (token === null && location.pathname !== "/sign-up") {
      navigate("/sign-in");
    }
  }, [navigate, location.pathname]);
  
  return (
      <Layout>
        <Routes>
          {AppRoutes.map((route, index) => {
            const { element, ...rest } = route;
            return <Route key={index} {...rest} element={element} />;
          })}
        </Routes>
      </Layout>
  );
};

export default App;
