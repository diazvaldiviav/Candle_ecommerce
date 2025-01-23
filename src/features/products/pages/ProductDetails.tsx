// src/features/products/components/ProductDetails/ProductDetails.tsx
import React, { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  Button,
  Rating,
  Divider,
  IconButton,
  styled,
  MenuItem,
  Select,
  InputLabel,
  FormControl
} from '@mui/material';
import {
  FavoriteBorder,
  Favorite,
  ShoppingCart
} from '@mui/icons-material';
import { useAppDispatch, useAppSelector } from '../../../hooks';
import { fetchProductById } from '../slices/productSlice';
import { ProductColor, ProductSize } from '../component/services/types/product.types';
import { useParams } from 'react-router-dom';

// Styled components (mantienen los mismos estilos que antes)
const ProductContainer = styled(Box)(({ theme }) => ({
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(4),
  padding: theme.spacing(3),
  maxWidth: 1200,
  margin: '0 auto',
  [theme.breakpoints.up('md')]: {
    flexDirection: 'row',
  },
}));

const InfoSection = styled(Box)(({ theme }) => ({
  flex: '0 0 50%',
  display: 'flex',
  flexDirection: 'column',
  gap: theme.spacing(3),
}));

const ActionButtons = styled(Box)(({ theme }) => ({
  display: 'flex',
  gap: theme.spacing(2),
  marginTop: theme.spacing(3),
}));

/* const ThumbnailContainer = styled(Box)(({ theme }) => ({
  display: 'flex',
  gap: theme.spacing(2),
  marginTop: theme.spacing(2),
})); */

/*const Thumbnail = styled('img')(({ theme }) => ({
  width: 80,
  height: 80,
  objectFit: 'cover',
  borderRadius: '4px',
  cursor: 'pointer',
  transition: 'border-color 0.2s',
  '&:hover': {
    borderColor: theme.palette.primary.main,
  },
}));*/

const ImageSection = styled(Box)(({ theme }) => ({
  flex: '0 0 50%',
  [theme.breakpoints.down('md')]: {
    width: '100%',
  },
}));

const ProductImage = styled('img')({
  width: '100%',
  height: 'auto',
  objectFit: 'cover',
  borderRadius: '8px',
});

const SelectWrapper = styled(FormControl)(({ theme }) => ({
  minWidth: 120,
  marginBottom: theme.spacing(2),
}));


// ... (resto de los styled components igual que antes)


const ProductDetails: React.FC = () => {
  const dispatch = useAppDispatch();
  const { selectedProduct: product, loading, error } = useAppSelector(state => state.products);
 // const [selectedImage, setSelectedImage] = useState(0);
 // const [quantity, setQuantity] = useState(1);
  const [isFavorite, setIsFavorite] = useState(false);
  const [selectedColor, setSelectedColor] = useState('');
  const [selectedSize, setSelectedSize] = useState('');
  const { id } = useParams<{ id: string }>();
 // const [selectedAroma, setSelectedAroma] = useState('');

  useEffect(() => {
    dispatch(fetchProductById(Number(id)));
  }, [dispatch, id]);

  if (loading) return <Box>Cargando...</Box>;
  if (error) return <Box>Error: {error}</Box>;
  if (!product) return <Box>Producto no encontrado</Box>;

  return (
    <ProductContainer>
      <ImageSection>
        <ProductImage
          src={product.imageUrl} // luego hay que agregar el array de imagenes
          alt={product.name}
        />
        {/* <ThumbnailContainer>
          {product.images?.map((image, index) => (
            <Thumbnail
              key={index}
              src={image}
              alt={`${product.name}-${index}`}
              onClick={() => setSelectedImage(index)}
              sx={{
                border: `2px solid ${selectedImage === index ? theme.palette.primary.main : 'transparent'}`
              }}
            />
          ))}
        </ThumbnailContainer> */}
      </ImageSection>

      <InfoSection>
        <Typography variant="h4" component="h1">
          {product.name}
        </Typography>

        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
          <Typography variant="h5" color="primary">
            ${product.price.toFixed(2)}
          </Typography>
          {/* {product.discount > 0 && (
            <Typography variant="body1" color="text.secondary" sx={{ textDecoration: 'line-through' }}>
              ${(product.price * (1 + product.discount)).toFixed(2)}
            </Typography>
          )} */}
        </Box>

        <Box>
          <Rating value={5} readOnly />
          <Typography variant="body2" color="text.secondary">
            {5} reseñas
          </Typography>
        </Box>

        <Typography variant="body1">
          {product.description}
        </Typography>

        <Divider />

        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
          <Typography variant="h6" gutterBottom>
            Personalización
          </Typography>
          
          <SelectWrapper>
            <InputLabel id="color-label">Color</InputLabel>
            <Select
              labelId="color-label"
              value={selectedColor}
              label="Color"
              onChange={(e) => setSelectedColor(e.target.value)}
            >
              {product.productColors.map((color: ProductColor) => (
                <MenuItem key={color.colorId} value={color.colorId}>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <Box
                      sx={{
                        width: 20,
                        height: 20,
                        borderRadius: '50%',
                        backgroundColor: color.hexCode,
                        border: '1px solid #ddd'
                      }}
                    />
                    <Typography>{color.colorName}</Typography>
                  </Box>
                </MenuItem>
              ))}
            </Select>
          </SelectWrapper>

          <SelectWrapper>
            <InputLabel id="size-label">Tamaño</InputLabel>
            <Select
              labelId="size-label"
              value={selectedSize}
              label="Tamaño"
              onChange={(e) => setSelectedSize(e.target.value)}
            >
              {product.productSizes.map((size: ProductSize) => (
                <MenuItem key={size.sizeId} value={size.sizeId}>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', width: '100%' }}>
                    <Typography>{size.sizeName}</Typography>
                  </Box>
                </MenuItem>
              ))}
            </Select>
            </SelectWrapper>

        {/*  <SelectWrapper>
            <InputLabel id="aroma-label">Aroma</InputLabel>
            <Select
              labelId="aroma-label"
              value={selectedAroma}
              label="Aroma"
              onChange={(e) => setSelectedAroma(e.target.value)}
            >
              {product.productAromas.map((aroma) => (
                <MenuItem key={aroma.id} value={aroma.id}>
                  <Typography>{aroma.name}</Typography>
                </MenuItem>
              ))}
            </Select>
          </SelectWrapper> */}
        </Box>

        <ActionButtons>
          <Button
            variant="contained"
            color="primary"
            startIcon={<ShoppingCart />}
            onClick={() => {/* Implementar añadir al carrito */}}
            size="large"
            fullWidth
          >
            Añadir al carrito
          </Button>
          <IconButton
            onClick={() => setIsFavorite(!isFavorite)}
            color={isFavorite ? 'primary' : 'default'}
          >
            {isFavorite ? <Favorite /> : <FavoriteBorder />}
          </IconButton>
        </ActionButtons>
      </InfoSection>
    </ProductContainer>
  );
};

export default ProductDetails;