import { Box, Container, Typography, Button, CircularProgress } from '@mui/material';
import { motion } from 'framer-motion';
import { styled } from '@mui/material/styles';
import { useNavigate } from 'react-router-dom';
import { useEffect } from 'react';
import { Product } from '../../../../features/products/component/services/types/product.types';
import { useAppSelector } from '../../../../hooks/useAppSelector';
import { useAppDispatch } from '../../../../hooks/useAppDispatch';
import { fetchProducts } from '../../../../features/products/slices/productSlice';

// Styled Components

const ProductsContainer = styled(Box)(({ theme }) => ({
  display: 'flex',
  gap: theme.spacing(3),
  flexWrap: 'wrap',
  justifyContent: 'center',
}));

const ProductCard = styled(motion.div)(({ theme }) => ({
  backgroundColor: 'white',
  borderRadius: theme.spacing(2),
  overflow: 'hidden',
  width: '280px',
  boxShadow: '0 4px 12px rgba(0,0,0,0.1)',
  transition: 'all 0.3s ease-in-out',
  display: 'flex',
  flexDirection: 'column',
}));

const ProductImage = styled('div')<{ image: string }>(({ image }) => ({
  width: '100%',
  height: '280px',
  backgroundImage: `url(${image})`,
  backgroundSize: 'cover',
  backgroundPosition: 'center',
}));

const ProductContent = styled(Box)(({ theme }) => ({
  padding: theme.spacing(2),
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(1),
  flexGrow: 1,
}));

const PriceTag = styled(Typography)(({ theme }) => ({
  color: theme.palette.primary.main,
  fontWeight: 600,
  fontSize: '1.5rem',
}));

// Animation variants
const containerVariants = {
  hidden: { opacity: 0 },
  visible: {
    opacity: 1,
    transition: {
      staggerChildren: 0.1
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

const ButtonsContainer = styled(Box)(({ theme }) => ({
    display: 'flex',
    gap: theme.spacing(1),
    marginTop: theme.spacing(2),
  }));
  
  const ActionButton = styled(Button)(({ theme }) => ({
    flex: 1,
    padding: theme.spacing(1),
    fontWeight: 600,
    textTransform: 'none',
    fontFamily: "'Montserrat', sans-serif",
  }));
  
  
  const DetailsButton = styled(ActionButton)(({ theme }) => ({
    backgroundColor: 'white',
    color: theme.palette.primary.main,
    width: '100%',
    border: `1px solid ${theme.palette.primary.main}`,
    '&:hover': {
      backgroundColor: 'rgba(107, 70, 193, 0.04)',
    },
  }));

 

export const ProductHome = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { products, loading, error } = useAppSelector(state => state.products);


 {/* const handleAddToCart = (productId: number) => {
    // Aquí irá la lógica para añadir al carrito
    console.log('Añadido al carrito:', productId);
  }; */}

  useEffect(() => {
    dispatch(fetchProducts());
  }, [dispatch]);

 

  if (loading) return <CircularProgress />;
  if (error) return <div>Error: {error}</div>;

  const firstThreeProducts = products.slice(0, 3);
  


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
          Productos Destacados
        </Typography>

        <motion.div
          variants={containerVariants}
          initial="hidden"
          whileInView="visible"
          viewport={{ once: true }}
        >
          <ProductsContainer>
            {firstThreeProducts?.map((product: Product) => (
              <ProductCard
                key={product.id}
                variants={cardVariants}
                whileHover={{ 
                  scale: 1.03,
                  boxShadow: '0 8px 16px rgba(0,0,0,0.2)'
                }}
              >
                <ProductImage 
                  image={product.imageUrl}
                  onClick={() => navigate(`/products/${product.id}`)}
                  style={{ cursor: 'pointer' }}
                />
                <ProductContent>
                  <Typography
                    variant="h6"
                    sx={{
                      fontWeight: 600,
                      color: 'primary.dark',
                      fontFamily: "'Montserrat', sans-serif",
                    }}
                  >
                    {product.name}
                  </Typography>
                  <Typography
                    variant="body2"
                    sx={{
                      color: 'text.secondary',
                      flexGrow: 1,
                    }}
                  >
                    {product.description}
                  </Typography>
                  <PriceTag>
                    ${product.price.toFixed(2)}
                  </PriceTag>
                  <ButtonsContainer>
                    <DetailsButton
                      variant="outlined"
                      onClick={() => navigate(`/products/${product.id}`)}
                      startIcon={
                        <motion.div
                          whileHover={{ x: 5 }}
                          transition={{ type: "spring", stiffness: 400 }}
                        >
                          →
                        </motion.div>
                      }
                    >
                      Ver Detalles
                    </DetailsButton>
                  </ButtonsContainer>
                </ProductContent>
              </ProductCard>
            ))}
          </ProductsContainer>
        </motion.div>
      </Container>
    </Box>
  );
};