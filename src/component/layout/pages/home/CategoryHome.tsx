import { Box, Container, Typography, Button, CircularProgress } from '@mui/material';
import { motion } from 'framer-motion';
import { styled } from '@mui/material/styles';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../../../hooks';
import { fetchCategories } from '../../../../features/categories/types/slices/categorySlice';
import { useEffect } from 'react';
import { Category } from '../../../../features/categories/types/category.types';

// Styled Components
const CategoriesContainer = styled(Box)(({ theme }) => ({
  display: 'flex',
  gap: theme.spacing(4),
  [theme.breakpoints.down('md')]: {
    flexDirection: 'column',
  },
}));

const CategoryCard = styled(motion.div)(({ theme }) => ({
  position: 'relative',
  borderRadius: theme.spacing(2),
  overflow: 'hidden',
  height: 400,
  cursor: 'pointer',
  boxShadow: '0 4px 12px rgba(0,0,0,0.1)',
  transition: 'all 0.3s ease-in-out',
  flex: 1,
  minWidth: 0, // Importante para flexbox
  [theme.breakpoints.down('md')]: {
    height: 300,
  },
}));

const CategoryImage = styled('div')<{ image: string }>(({ image }) => ({
  position: 'absolute',
  top: 0,
  left: 0,
  right: 0,
  bottom: 0,
  backgroundImage: `url(${image})`,
  backgroundSize: 'cover',
  backgroundPosition: 'center',
  '&::before': {
    content: '""',
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    background: 'linear-gradient(to bottom, rgba(0,0,0,0.2) 0%, rgba(0,0,0,0.6) 100%)',
  },
}));

const CategoryContent = styled(Box)(({ theme }) => ({
  position: 'relative',
  padding: theme.spacing(3),
  height: '100%',
  display: 'flex',
  flexDirection: 'column',
  justifyContent: 'space-between',
  color: 'white',
}));

const StyledButton = styled(Button)(({ theme }) => ({
  backgroundColor: 'rgba(255, 255, 255, 0.9)',
  color: theme.palette.primary.main,
  marginBottom: '30px',
  padding: theme.spacing(1, 4),
  '&:hover': {
    backgroundColor: theme.palette.primary.main,
    color: 'white',
  },
}));


const containerVariants = {
  hidden: { opacity: 0 },
  visible: {
    opacity: 1,
    transition: {
      staggerChildren: 0.2
    }
  }
};

const cardVariants = {
  hidden: { opacity: 0, y: 20 },
  visible: {
    opacity: 1,
    y: 0,
    transition: {
      duration: 0.5
    }
  }
};

export const CategoryHome = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { categories, loading, error } = useAppSelector(state => state.categories);


  useEffect(() => {
    dispatch(fetchCategories());
  }, [dispatch]);

  // Log cuando el componente recibe nuevos datos
  useEffect(() => {
    console.log('Categorías actualizadas:', categories);
  }, [categories]);

  if (loading) {
    console.log('Cargando categorías...');
    return (
      <Box display="flex" justifyContent="center" p={2}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    console.error('Error al cargar categorías:', error);
    return (
      <Typography color="error" p={2}>
        {error}
      </Typography>
    );
  }



  return (
    <Box sx={{ py: 8, backgroundColor: 'background.default' }}>
      <Container maxWidth="lg">
        <Typography
          variant="h2"
          component="h2"
          align="center"
          sx={{
            mb: 6,
            fontWeight: 700,
            color: 'primary.main',
            fontFamily: "'Montserrat', sans-serif",
          }}
        >
          Categorías Destacadas
        </Typography>

        <motion.div
          variants={containerVariants}
          initial="hidden"
          whileInView="visible"
          viewport={{ once: true }}
        >
          <CategoriesContainer>
            {categories.map((category: Category) => (
              <CategoryCard
                key={category.id}
                variants={cardVariants}
                whileHover={{ 
                  scale: 1.02,
                  boxShadow: '0 8px 16px rgba(0,0,0,0.2)'
                }}
              >
                <CategoryImage image={category.imageUrl} />
                <CategoryContent>
                  <Box>
                    <Typography
                      variant="h3"
                      component="h3"
                      sx={{
                        fontWeight: 700,
                        mb: 2,
                        fontFamily: "'Montserrat', sans-serif",
                      }}
                    >
                      {category.name}
                    </Typography>
                    <Typography
                      variant="h5"
                      sx={{
                        fontWeight: 500,
                        opacity: 0.9,
                        fontFamily: "'Montserrat', sans-serif",
                      }}
                    >
                      {category.description}
                    </Typography>
                  </Box>
                  <StyledButton
                    variant="contained"
                    size="large"
                    onClick={() => navigate(`/products`)}
                    sx={{ mt: 2 }}
                  >
                    Ver Productos
                  </StyledButton>
                </CategoryContent>
              </CategoryCard>
            ))}
          </CategoriesContainer>
        </motion.div>
      </Container>
    </Box>
  );
};