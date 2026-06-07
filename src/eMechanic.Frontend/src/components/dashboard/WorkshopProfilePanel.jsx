import React from "react";
import {
  Box,
  Flex,
  Button,
  Heading,
  SimpleGrid,
  VStack,
  Text,
  Input,
  Separator,
  Field,
  Textarea,
} from "@chakra-ui/react";

export const WorkshopProfilePanel = ({
  displayName,
  setDisplayName,
  email,
  setEmail,
  phone,
  setPhone,
  city,
  setCity,
  address,
  setAddress,
  description,
  setDescription,
  submitting,
  onSubmit,
}) => {
  return (
    <VStack gap={6} align="stretch">
      <Box>
        <Heading size="2xl" fontWeight="black" tracking="tight" _dark={{ color: "white" }}>
          Profil Warsztatu
        </Heading>
        <Text color="gray.500" _dark={{ color: "gray.400" }} fontSize="md" mt={1}>
          Zarządzaj informacjami publicznymi swojego warsztatu, które widzą klienci w wyszukiwarce.
        </Text>
      </Box>

      <Box
        p={6}
        bg="white"
        _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
        borderWidth="1px"
        borderColor="gray.200"
        rounded="2xl"
        boxShadow="0 15px 35px -5px rgba(249, 115, 22, 0.04)"
        as="form"
        onSubmit={onSubmit}
      >
        <VStack align="stretch" gap={5}>
          <SimpleGrid columns={{ base: 1, md: 2 }} gap={4}>
            <Field.Root required>
              <Field.Label fontWeight="semibold">Nazwa warsztatu</Field.Label>
              <Input
                placeholder="np. Auto Serwis Kowalski"
                value={displayName}
                onChange={(e) => setDisplayName(e.target.value)}
                _dark={{ bg: "rgb(15, 23, 42)" }}
                rounded="xl"
                size="md"
                _focus={{
                  borderColor: "orange.500",
                  borderWidth: "2px",
                  outline: "none",
                }}
              />
            </Field.Root>

            <Field.Root required>
              <Field.Label fontWeight="semibold">Adres Email (Login)</Field.Label>
              <Input
                type="email"
                placeholder="np. kontakt@serwiskowalski.pl"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                _dark={{ bg: "rgb(15, 23, 42)" }}
                rounded="xl"
                size="md"
                _focus={{
                  borderColor: "orange.500",
                  borderWidth: "2px",
                  outline: "none",
                }}
              />
            </Field.Root>
          </SimpleGrid>

          <SimpleGrid columns={{ base: 1, md: 3 }} gap={4}>
            <Field.Root required>
              <Field.Label fontWeight="semibold">Miasto</Field.Label>
              <Input
                placeholder="np. Warszawa"
                value={city}
                onChange={(e) => setCity(e.target.value)}
                _dark={{ bg: "rgb(15, 23, 42)" }}
                rounded="xl"
                size="md"
                _focus={{
                  borderColor: "orange.500",
                  borderWidth: "2px",
                  outline: "none",
                }}
              />
            </Field.Root>

            <Field.Root required>
              <Field.Label fontWeight="semibold">Adres (Ulica i nr)</Field.Label>
              <Input
                placeholder="np. Kolejowa 12"
                value={address}
                onChange={(e) => setAddress(e.target.value)}
                _dark={{ bg: "rgb(15, 23, 42)" }}
                rounded="xl"
                size="md"
                _focus={{
                  borderColor: "orange.500",
                  borderWidth: "2px",
                  outline: "none",
                }}
              />
            </Field.Root>

            <Field.Root>
              <Field.Label fontWeight="semibold">Telefon kontaktowy</Field.Label>
              <Input
                placeholder="np. +48600700800"
                value={phone}
                onChange={(e) => setPhone(e.target.value)}
                _dark={{ bg: "rgb(15, 23, 42)" }}
                rounded="xl"
                size="md"
                _focus={{
                  borderColor: "orange.500",
                  borderWidth: "2px",
                  outline: "none",
                }}
              />
            </Field.Root>
          </SimpleGrid>

          <Field.Root>
            <Field.Label fontWeight="semibold">Opis warsztatu i specjalizacje</Field.Label>
            <Textarea
              placeholder="Opisz swój warsztat, w jakich markach pojazdów się specjalizujecie, jakie usługi oferujecie oraz podaj godziny otwarcia..."
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              _dark={{ bg: "rgb(15, 23, 42)" }}
              rounded="xl"
              rows={5}
              _focus={{
                borderColor: "orange.500",
                borderWidth: "2px",
                outline: "none",
              }}
            />
          </Field.Root>

          <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} mt={2} />

          <Flex justify="flex-end">
            <Button
              type="submit"
              colorPalette="orange"
              loading={submitting}
              rounded="xl"
              px={8}
              fontWeight="bold"
              shadow="md"
              _hover={{ transform: "translateY(-1px)", shadow: "lg" }}
            >
              Zapisz Zmiany
            </Button>
          </Flex>
        </VStack>
      </Box>
    </VStack>
  );
};

export default WorkshopProfilePanel;
