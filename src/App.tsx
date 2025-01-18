import { ThemeProvider } from '@mui/material/styles';
import { theme } from './config/theme';
import { RouterProvider } from 'react-router-dom';
import { router } from './config/router';
import { Suspense } from 'react';
import { LoadingPage } from './component/common/loadingPage';

function App() {
  return (
    <ThemeProvider theme={theme}>
      <Suspense fallback={<LoadingPage />}>
        <RouterProvider router={router} />
      </Suspense>
    </ThemeProvider>
  );
}

export default App;