// src/features/products/components/FilterToolbar.tsx
import { 
  Box, 
  TextField, 
  MenuItem,
  styled,
  Typography
} from '@mui/material';
import { Search } from '@mui/icons-material';

const ToolbarContainer = styled(Box)(({ theme }) => ({
  display: 'flex',
  gap: theme.spacing(2),
  marginBottom: theme.spacing(3),
  alignItems: 'center',
  flexWrap: 'wrap',
  [theme.breakpoints.down('sm')]: {
    flexDirection: 'column',
    alignItems: 'stretch',
  },
}));

const SearchContainer = styled('div')(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  padding: theme.spacing(1),
  paddingLeft: theme.spacing(2),
  backgroundColor: theme.palette.background.paper,
  borderRadius: theme.shape.borderRadius,
  border: `1px solid ${theme.palette.divider}`,
  flex: 1,
  '&:hover': {
    borderColor: theme.palette.text.primary,
  },
  '&:focus-within': {
    borderColor: theme.palette.primary.main,
    boxShadow: `0 0 0 2px ${theme.palette.primary.main}25`,
  },
}));

const SearchInput = styled('input')(({ theme }) => ({
  border: 'none',
  outline: 'none',
  width: '100%',
  padding: theme.spacing(1),
  backgroundColor: 'transparent',
  fontSize: '1rem',
  color: theme.palette.text.primary,
  '&::placeholder': {
    color: theme.palette.text.secondary,
  },
}));

const SearchIcon = styled(Search)(({ theme }) => ({
  color: theme.palette.text.secondary,
  marginRight: theme.spacing(1),
}));

const SortSelect = styled(TextField)(({ theme }) => ({
  minWidth: 200,
  '& .MuiSelect-select': {
    padding: theme.spacing(1.5),
  },
}));

const sortOptions = [
  { value: 'newest', label: 'Más recientes' },
  { value: 'price_low', label: 'Precio: Menor a Mayor' },
  { value: 'price_high', label: 'Precio: Mayor a Menor' },
  { value: 'name_asc', label: 'Nombre: A-Z' },
  { value: 'name_desc', label: 'Nombre: Z-A' },
];

interface FilterToolbarProps {
  onSearch: (query: string) => void;
  onSort: (option: string) => void;
  totalProducts: number;
}

const FilterToolbar = ({ onSearch, onSort, totalProducts }: FilterToolbarProps) => {
  return (
    <ToolbarContainer>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, width: '100%' }}>
        <SearchContainer>
          <SearchIcon />
          <SearchInput
            placeholder="Buscar productos..."
            onChange={(e) => onSearch(e.target.value)}
            aria-label="buscar productos"
          />
        </SearchContainer>
        
        <SortSelect
          select
          size="small"
          defaultValue="newest"
          onChange={(e) => onSort(e.target.value)}
          variant="outlined"
        >
          {sortOptions.map((option) => (
            <MenuItem key={option.value} value={option.value}>
              {option.label}
            </MenuItem>
          ))}
        </SortSelect>
      </Box>
      
      <Typography variant="body2" color="text.secondary">
        {totalProducts} productos encontrados
      </Typography>
    </ToolbarContainer>
  );
};

export default FilterToolbar;