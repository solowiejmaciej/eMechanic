import { Provider } from "@/components/ui/provider"
import { Box, Button, Heading, Text, VStack } from "@chakra-ui/react"
import ChakraNavbar from "@/components/layout/ChakraNavbar"
import ChakraHero from "../features/landing/ChakraHero"
import ChakraFeature from "../features/landing/ChakraFeature"
import ChakraFooter from "../components/layout/ChakraFooter"
export default function ChakraLandingPage() {
  return (
    
    <Provider>
      <Box minH="100vh" w="full" bg="gray.100"  _dark={{ bg: "#0F172A" }} >
        <VStack pt={{ base: "100px", lg: "300px" }}>
          <ChakraNavbar />
          <ChakraHero />
          <ChakraFeature />
          <ChakraFooter />
        </VStack>
        </Box>
    </Provider>
  )
}