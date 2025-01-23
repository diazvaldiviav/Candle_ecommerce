import { Box } from '@mui/material';
import { Outlet } from 'react-router-dom';
import { Navbar } from './Navbar/NavBar';
import { Footer } from './footer/footer';

export const MainLayout = () => {
  return (
    <Box>
      <Navbar />
      <Box component="main" sx={{ pt: 8 }}>
        <Outlet />
      </Box>
      {/* Footer */}
      <Footer />
    </Box>
  );
};