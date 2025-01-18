import { Box } from '@mui/material';
import { Outlet } from 'react-router-dom';

export const MainLayout = () => {
  return (
    <Box>
      {/* Navbar */}
      <Box component="main">
        <Outlet />
      </Box>
      {/* Footer */}
    </Box>
  );
};