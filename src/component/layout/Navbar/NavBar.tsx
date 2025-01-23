import { useState } from 'react';
import { 
  AppBar,
  Box,
  Toolbar,
  IconButton,
  Typography,
  Badge,
  Menu,
  MenuItem,
  useTheme,
  useMediaQuery,
  Drawer,
  List,
  ListItem,
  ListItemText,
  Container,
} from '@mui/material';
import {
  ShoppingCart as ShoppingCartIcon,
  Person as PersonIcon,
  Menu as MenuIcon,
  Close as CloseIcon
} from '@mui/icons-material';
import { styled } from '@mui/material/styles';
import { Link, useNavigate } from 'react-router-dom';







const NavLink = styled(Link)(({ theme }) => ({
  color: theme.palette.primary.main,
  textDecoration: 'none',
  padding: theme.spacing(1, 2),
  borderRadius: theme.shape.borderRadius,
  '&:hover': {
    backgroundColor: theme.palette.primary.main,
    color: theme.palette.common.white,
  },
}));

export const Navbar = () => {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('md'));
  const navigate = useNavigate();

  const handleProfileMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const menuId = 'primary-search-account-menu';
  const renderMenu = (
    <Menu
      anchorEl={anchorEl}
      id={menuId}
      keepMounted
      open={Boolean(anchorEl)}
      onClose={handleMenuClose}
    >
      <MenuItem onClick={() => navigate('/auth/login')}>Iniciar Sesión</MenuItem>
      <MenuItem onClick={() => navigate('/auth/register')}>Registrarse</MenuItem>
    </Menu>
  );

  const navigationLinks = [
    { text: 'Inicio', path: '/' },
    { text: 'Productos', path: '/products' },
    { text: 'Sobre Nosotros', path: '/about' },
    { text: 'Contacto', path: '/contact' },
  ];

  return (
    <Box sx={{ flexGrow: 1 }}>
      <AppBar position="fixed" color="inherit" elevation={1}>
        <Container maxWidth="xl">
          <Toolbar>
            {/* Logo */}
            <Typography
              variant="h6"
              noWrap
              component={Link}
              to="/"
              sx={{
                display: 'flex',
                fontFamily: 'monospace',
                fontWeight: 700,
                letterSpacing: '.3rem',
                color: 'primary.main',
                textDecoration: 'none',
              }}
            >
              CANDLE
            </Typography>

            {isMobile ? (
               <Box sx={{ display: 'flex', ml: 'auto', gap: 1 }}>
               <IconButton
                 size="large"
                 color="primary"
                 onClick={() => navigate('/cart')}
               >
                 <Badge badgeContent={4} color="error">
                   <ShoppingCartIcon />
                   <PersonIcon />
                 </Badge>
               </IconButton>
               <IconButton
                 color="primary"
                 aria-label="open drawer"
                 edge="start"
                 onClick={() => setMobileMenuOpen(true)}
               >
                 <MenuIcon />
               </IconButton>
             </Box>
            ) : (
              <>
                {/* Navigation Links */}
                <Box sx={{ display: 'flex', mx: 4 }}>
                  {navigationLinks.map((link) => (
                    <NavLink key={link.path} to={link.path}>
                      {link.text}
                    </NavLink>
                  ))}
                </Box>

              
                {/* Icons */}
                <Box sx={{ display: 'flex', ml: 'auto' }}>
                  <IconButton
                    size="large"
                    edge="end"
                    aria-label="account of current user"
                    aria-controls={menuId}
                    aria-haspopup="true"
                    onClick={handleProfileMenuOpen}
                    color="primary"
                  >
                    <PersonIcon />
                  </IconButton>
                  <IconButton 
                    size="large" 
                    color="primary"
                    onClick={() => navigate('/cart')}
                  >
                    <Badge badgeContent={4} color="error">
                      <ShoppingCartIcon />
                    </Badge>
                  </IconButton>
                </Box>
              </>
            )}
          </Toolbar>
        </Container>
      </AppBar>

      {/* Mobile Menu */}
      <Drawer
        anchor="right"
        open={mobileMenuOpen}
        onClose={() => setMobileMenuOpen(false)}
      >
        <Box
          sx={{
            width: 250,
            pt: 2,
            height: '100%',
            backgroundColor: 'background.paper',
          }}
          role="presentation"
        >
          <IconButton
            onClick={() => setMobileMenuOpen(false)}
            sx={{ ml: 2, mb: 2 }}
          >
            <CloseIcon />
          </IconButton>
          <List>
            {navigationLinks.map((link) => (
              <ListItem 
                key={link.path}
                component={Link}
                to={link.path}
                onClick={() => setMobileMenuOpen(false)}
              >
                <ListItemText primary={link.text} />
              </ListItem>
            ))}
          </List>
          
        </Box>
      </Drawer>

      {renderMenu}
      
    </Box>
  );
};