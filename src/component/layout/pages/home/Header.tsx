import { useEffect, useState } from "react";
import {
  Typography,
  Button,
  Container,
  useTheme,
  useMediaQuery,
} from "@mui/material";
import { motion, useScroll, useTransform } from "framer-motion";
import { styled } from "@mui/material/styles";
import { useNavigate } from "react-router-dom";

// Styled components
const HeaderContainer = styled(motion.div)(({ theme }) => ({
  position: "relative",
  height: "80vh",
  width: "100%",
  overflow: "hidden",
  [theme.breakpoints.down("md")]: {
    height: "60vh",
  },
}));

const BackgroundImage = styled(motion.div)({
  position: "absolute",
  top: 0,
  left: 0,
  right: 0,
  bottom: 0,
  backgroundImage: 'url("https://images.unsplash.com/photo-1603006905003-be475563bc59?q=80&w=1974&auto=format&fit=crop")', // Asegúrate de tener esta imagen
  backgroundSize: "cover",
  backgroundPosition: "center",
  "&::before": {
    content: '""',
    position: "absolute",
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    background: `linear-gradient(
      to bottom,
      rgba(255,255,255,0.1) 0%,
      rgba(107,70,193,0.3) 100%
    )`,
  },
});

const ContentContainer = styled(Container)(({ theme }) => ({
  height: "100%",
  display: "flex",
  flexDirection: "column",
  justifyContent: "flex-end",
  alignItems: "center",
  paddingBottom: theme.spacing(8),
  position: "relative",
  zIndex: 1,
  [theme.breakpoints.down("md")]: {
    paddingBottom: theme.spacing(4),
  },
}));

const StyledButton = styled(Button)(({ theme }) => ({
  backgroundColor: "white",
  color: theme.palette.primary.main,
  padding: theme.spacing(1.5, 4),
  fontSize: "1.1rem",
  fontWeight: 600,
  marginTop: theme.spacing(3),
  "&:hover": {
    backgroundColor: theme.palette.primary.main,
    color: "white",
  },
  [theme.breakpoints.down("md")]: {
    padding: theme.spacing(1, 3),
    fontSize: "1rem",
  },
}));

export const Header = () => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("md"));
  const { scrollY } = useScroll();
  const [isVisible, setIsVisible] = useState(true);
  const navigate = useNavigate();

  // Parallax effect
  const y = useTransform(scrollY, [0, 500], [0, 150]);
  const opacity = useTransform(scrollY, [0, 300], [1, 0]);

  // Hide header when scrolling down
  useEffect(() => {
    const unsubscribe = scrollY.onChange((latest) => {
      setIsVisible(latest < 100);
    });
    return () => unsubscribe();
  }, [scrollY]);

  return (

    
    <motion.div
    initial={{ opacity: 0 }}
    animate={{ opacity: isVisible ? 1 : 0 }}
    transition={{ duration: 0.8 }}
    style={{
        position: 'relative',
        zIndex: isVisible ? 1 : 0 // También podemos controlar el z-index
      }}
  >
    <HeaderContainer>
      <BackgroundImage
         style={{ 
            y, 
            opacity: isVisible ? opacity : 0 // Combinamos con el efecto parallax
          }}
        initial={{ scale: 1.1 }}
        animate={{ scale: 1 }}
        transition={{ duration: 0.8 }}
      />
      <ContentContainer maxWidth="lg">
        <motion.div
          initial={{ y: 50, opacity: 0 }}
          animate={{ 
            y: 0, 
            opacity: isVisible ? 1 : 0 // El contenido también se desvanece
          }}
          transition={{ delay: 0.3, duration: 0.8 }}
          style={{
            textAlign: 'center',
            maxWidth: '800px',
          }}
        >
          <Typography
            variant={isMobile ? 'h4' : 'h2'}
            component="h1"
            sx={{
              color: 'white',
              fontWeight: 700,
              fontFamily: 'Montserrat, sans-serif',
              textShadow: '2px 2px 4px rgba(0,0,0,0.2)',
              mb: 2,
            }}
          >
             Descubre la Elegancia Moderna
            </Typography>
            <Typography
              variant={isMobile ? 'h6' : 'h5'}
              sx={{
                color: 'white',
                fontWeight: 500,
                fontFamily: 'Montserrat, sans-serif',
                textShadow: '1px 1px 2px rgba(0,0,0,0.2)',
              }}
            >
              20% de descuento en la Nueva Colección
            </Typography>
            <motion.div
              whileHover={{ scale: 1.05 }}
              whileTap={{ scale: 0.95 }}
            >
              <StyledButton
                variant="contained"
                onClick={() => navigate('/products')}
              >
                Compra Ahora
              </StyledButton>
            </motion.div>
          </motion.div>
        </ContentContainer>
      </HeaderContainer>
    </motion.div>
  );
};
