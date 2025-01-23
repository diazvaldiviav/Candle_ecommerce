import {
  Box,
  Container,
  Typography,
  TextField,
  Button,
  IconButton,
  Link,
  styled,
} from "@mui/material";
import {
  Facebook,
  Instagram,
  Twitter,
  Phone,
  Email,
  LocationOn,
} from "@mui/icons-material";

// Styled Components
const FooterContainer = styled(Box)(({ theme }) => ({
  backgroundColor: theme.palette.primary.dark,
  color: "white",
  paddingTop: theme.spacing(6),
  paddingBottom: theme.spacing(4),
}));

const FooterContent = styled(Box)(({ theme }) => ({
  display: "flex",
  flexWrap: "wrap",
  gap: theme.spacing(4),
  marginBottom: theme.spacing(4),
  [theme.breakpoints.down("sm")]: {
    flexDirection: "column",
  },
}));

const FooterSection = styled(Box)(({ theme }) => ({
  flex: "1 1 250px",
  minWidth: 250,
  marginBottom: theme.spacing(3),
}));

const SocialIcon = styled(IconButton)(({ theme }) => ({
  color: "white",
  marginRight: theme.spacing(1),
  "&:hover": {
    color: theme.palette.secondary.main,
  },
}));

const FooterLink = styled(Link)(({ theme }) => ({
  color: "white",
  textDecoration: "none",
  "&:hover": {
    color: theme.palette.secondary.main,
  },
  display: "block",
  marginBottom: theme.spacing(1),
}));

const NewsletterInput = styled(TextField)(({ theme }) => ({
  "& .MuiOutlinedInput-root": {
    color: "white",
    "& fieldset": {
      borderColor: "rgba(255, 255, 255, 0.23)",
    },
    "&:hover fieldset": {
      borderColor: "white",
    },
    "&.Mui-focused fieldset": {
      borderColor: theme.palette.secondary.main,
    },
  },
  "& .MuiInputLabel-root": {
    color: "rgba(255, 255, 255, 0.7)",
  },
}));

const SubscribeButton = styled(Button)(({ theme }) => ({
  backgroundColor: "white",
  color: theme.palette.primary.main,
  "&:hover": {
    backgroundColor: theme.palette.secondary.main,
    color: "white",
  },
  marginTop: theme.spacing(2),
}));

const ContactItem = styled(Box)(({ theme }) => ({
  display: "flex",
  alignItems: "center",
  marginBottom: theme.spacing(2),
  "& svg": {
    marginRight: theme.spacing(1),
  },
}));

export const Footer = () => {
  const handleSubscribe = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    // Implementar lógica de suscripción
  };

  return (
    <FooterContainer>
      <Container maxWidth="lg">
        <FooterContent>
          {/* Información de Contacto */}
          <FooterSection>
            <Typography variant="h6" gutterBottom>
              Contacto
            </Typography>
            <ContactItem>
              <Phone />
              <Typography>+1 234 567 890</Typography>
            </ContactItem>
            <ContactItem>
              <Email />
              <Typography>info@tutienda.com</Typography>
            </ContactItem>
            <ContactItem>
              <LocationOn />
              <Typography>123 Calle Principal, Ciudad</Typography>
            </ContactItem>
          </FooterSection>

          {/* Enlaces Rápidos */}
          <FooterSection>
            <Typography variant="h6" gutterBottom>
              Enlaces Rápidos
            </Typography>
            <FooterLink href="/about">Acerca de</FooterLink>
            <FooterLink href="/privacy">Política de Privacidad</FooterLink>
            <FooterLink href="/terms">Términos y Condiciones</FooterLink>
            <FooterLink href="/contact">Contacto</FooterLink>
          </FooterSection>

          {/* Redes Sociales */}
          <FooterSection>
            <Typography variant="h6" gutterBottom>
              Síguenos
            </Typography>
            <Box>
              <Link href="https://facebook.com" target="_blank">
                <SocialIcon>
                  <Facebook />
                </SocialIcon>
              </Link>
              <Link href="https://instagram.com" target="_blank">
                <SocialIcon>
                  <Instagram />
                </SocialIcon>
              </Link>
              <Link href="https://twitter.com" target="_blank">
                <SocialIcon>
                  <Twitter />
                </SocialIcon>
              </Link>
            </Box>
          </FooterSection>

          {/* Newsletter */}
          <FooterSection>
            <Typography variant="h6" gutterBottom>
              Newsletter
            </Typography>
            <form onSubmit={handleSubscribe}>
              <NewsletterInput
                fullWidth
                label="Email"
                variant="outlined"
                size="small"
                margin="normal"
              />
              <SubscribeButton type="submit" variant="contained" fullWidth>
                Suscribirse
              </SubscribeButton>
            </form>
          </FooterSection>
        </FooterContent>

        {/* Copyright */}
        <Box
          sx={{
            borderTop: "1px solid rgba(255, 255, 255, 0.1)",
            paddingTop: 3,
            textAlign: "center",
          }}
        >
          <Typography variant="body2" color="rgba(255, 255, 255, 0.7)">
            © {new Date().getFullYear()} Tu Tienda. Todos los derechos
            reservados.
          </Typography>
        </Box>
      </Container>
    </FooterContainer>
  );
};
