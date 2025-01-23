// src/components/products/ProductGrid.tsx
import { 
    Card, 
    CardMedia, 
    CardContent, 
    Typography, 
    CardActions, 
    Button,
    Box,
    Pagination,
    styled 
  } from '@mui/material';
  import { ShoppingCart } from '@mui/icons-material';
import { Product } from './services/types/product.types';
import { CircularProgress } from '@mui/material';
import { useNavigate } from 'react-router-dom';

const ProductsGrid = styled('div')(({ theme }) => ({
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))',
    gap: theme.spacing(3),
    padding: theme.spacing(2),
    [theme.breakpoints.down('sm')]: {
      gridTemplateColumns: 'repeat(auto-fill, minmax(240px, 1fr))',
    }
  }));
  
  const ProductCard = styled(Card)(({ theme }) => ({
    display: 'flex',
    flexDirection: 'column',
    height: '100%',
    transition: 'all 0.3s ease-in-out',
    '&:hover': {
      transform: 'translateY(-8px)',
      boxShadow: theme.shadows[8],
    },
    borderRadius: theme.spacing(2),
    overflow: 'hidden',
    background: theme.palette.background.paper,
  }));
  
  const ProductImage = styled(CardMedia)({
    paddingTop: '100%', // Aspect ratio 1:1
    backgroundSize: 'cover',
    backgroundPosition: 'center',
    position: 'relative',
    '&::after': {
      content: '""',
      position: 'absolute',
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      background: 'linear-gradient(180deg, rgba(0,0,0,0) 0%, rgba(0,0,0,0.1) 100%)',
      opacity: 0,
      transition: 'opacity 0.3s ease-in-out',
    },
    '&:hover::after': {
      opacity: 1,
    }
  });
  
  const ProductContent = styled(CardContent)(({ theme }) => ({
    flexGrow: 1,
    padding: theme.spacing(2),
  }));
  
  const PriceTypography = styled(Typography)(({ theme }) => ({
    color: theme.palette.primary.main,
    fontWeight: 'bold',
    fontSize: '1.25rem',
  }));

  interface ProductGridProps {
    products: Product[];
    loading: boolean;
    error: string | null;
    currentPage: number;
    totalPages: number;
    onPageChange: (page: number) => void;
  }
  
  const ProductGrid: React.FC<ProductGridProps> = ({ 
    products, 
    loading, 
    error, 
    currentPage, 
    totalPages,
    onPageChange,
  }: ProductGridProps) => {
    const navigate = useNavigate();
    if (loading) return <CircularProgress />;
    if (error) return <Box>Error al cargar los productos</Box>;
    if (!products || products.length === 0) return <Box>No hay productos disponibles</Box>;

    console.log('ProductGrid renderizado con productos:', products);
    return (
      <Box>
        <ProductsGrid>
          {products.map((product: Product) => (
            <ProductCard>
                <ProductImage
                  image={product.imageUrl}
                  title={product.name}
                />
                <ProductContent>
                  <Typography gutterBottom variant="h6" component="h2" noWrap>
                    {product.name}
                  </Typography>
                  <Typography
                    variant="body2"
                    color="text.secondary"
                    sx={{
                      overflow: 'hidden',
                      textOverflow: 'ellipsis',
                      display: '-webkit-box',
                      WebkitLineClamp: 2,
                      WebkitBoxOrient: 'vertical',
                    }}
                  >
                    {product.description}
                  </Typography>
                  <Box sx={{ mt: 2 }}>
                    <PriceTypography>
                      ${product.price.toFixed(2)}
                    </PriceTypography>
                  </Box>
                </ProductContent>
                <CardActions sx={{ justifyContent: 'space-between', px: 2, pb: 2 }}>
                  <Button 
                    size="small" 
                    color="primary"
                    variant="outlined"
                    onClick={() => navigate(`/products/${product.id}`)}
                  >
                    Ver más
                  </Button>
                  <Button
                    size="small"
                    color="primary"
                    variant="contained"
                    startIcon={<ShoppingCart />}
                    onClick={() => {/* Implementar añadir al carrito */}}
                  >
                    Añadir
                  </Button>
                </CardActions>
              </ProductCard>
            ))}
        </ProductsGrid>

        {totalPages > 1 && (
          <Box sx={{ mt: 4, display: 'flex', justifyContent: 'center' }}>
            <Pagination
              count={totalPages}
              page={currentPage}
              onChange={(_, page) => onPageChange(page)}
              color="primary"
            />
          </Box>
        )}
      </Box>
    );
  };
  
  export default ProductGrid;