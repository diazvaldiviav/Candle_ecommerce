import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import {
  Container,
  Grid,
  Typography,
  Box,
  Button,
  CircularProgress,
  Paper,
  Chip,
} from '@mui/material';

interface ProductDetail {
  id: number;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  colors: string[];
  sizes: string[];
}

const ProductDetail = () => {
  const { id } = useParams();
  const [product, setProduct] = useState<ProductDetail | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Aquí irá la llamada a la API
    // Por ahora usamos datos de ejemplo
    const mockProduct: ProductDetail = {
      id: Number(id),
      name: "Vela Lavanda",
      description: "Vela aromática con esencia de lavanda natural...",
      price: 19.99,
      imageUrl: "https://via.placeholder.com/400",
      colors: ["Púrpura", "Blanco"],
      sizes: ["Pequeño", "Mediano", "Grande"]
    };

    setTimeout(() => {
      setProduct(mockProduct);
      setLoading(false);
    }, 1000);
  }, [id]);

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (!product) {
    return (
      <Typography variant="h6" align="center">
        Producto no encontrado
      </Typography>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4 }}>
      <Paper elevation={3} sx={{ p: 3 }}>
        <Grid container spacing={4}>
          <Grid item xs={12} md={6}>
            <img
              src={product.imageUrl}
              alt={product.name}
              style={{ width: '100%', height: 'auto' }}
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <Typography variant="h4" component="h1" gutterBottom>
              {product.name}
            </Typography>
            <Typography variant="h5" color="primary" gutterBottom>
              ${product.price}
            </Typography>
            <Typography variant="body1" paragraph>
              {product.description}
            </Typography>
            
            <Box sx={{ mb: 2 }}>
              <Typography variant="subtitle1" gutterBottom>
                Colores disponibles:
              </Typography>
              <Box sx={{ display: 'flex', gap: 1 }}>
                {product.colors.map((color) => (
                  <Chip key={color} label={color} />
                ))}
              </Box>
            </Box>

            <Box sx={{ mb: 3 }}>
              <Typography variant="subtitle1" gutterBottom>
                Tamaños disponibles:
              </Typography>
              <Box sx={{ display: 'flex', gap: 1 }}>
                {product.sizes.map((size) => (
                  <Chip key={size} label={size} />
                ))}
              </Box>
            </Box>

            <Button
              variant="contained"
              size="large"
              fullWidth
              sx={{ mt: 2 }}
            >
              Agregar al Carrito
            </Button>
          </Grid>
        </Grid>
      </Paper>
    </Container>
  );
};

export default ProductDetail;