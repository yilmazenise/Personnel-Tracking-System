#include <SPI.h>
#include <MFRC522.h>

#define RST_PIN         9          
#define SS_1_PIN        10         
#define SS_2_PIN        8          

#define NR_OF_READERS   2
#define buzzer 4

byte ssPins[] = {SS_1_PIN, SS_2_PIN};

MFRC522 mfrc522[NR_OF_READERS];   

void setup() {

  pinMode(buzzer, OUTPUT);

  Serial.begin(9600); 
  while (!Serial);    

  SPI.begin();        

  for (uint8_t reader = 0; reader < NR_OF_READERS; reader++) {
    mfrc522[reader].PCD_Init(ssPins[reader], RST_PIN); 
  }
}

void loop() {

  for (uint8_t reader = 0; reader < NR_OF_READERS; reader++) {
   

    if (mfrc522[reader].PICC_IsNewCardPresent() && mfrc522[reader].PICC_ReadCardSerial()) {
      digitalWrite(buzzer, HIGH);
      delay(200);
      digitalWrite(buzzer, LOW);
      dump_byte_array(mfrc522[reader].uid.uidByte, mfrc522[reader].uid.size);
      MFRC522::PICC_Type piccType = mfrc522[reader].PICC_GetType(mfrc522[reader].uid.sak);
      

    
      mfrc522[reader].PICC_HaltA();
      
      mfrc522[reader].PCD_StopCrypto1();
    } 
}

void dump_byte_array(byte *buffer, byte bufferSize) {
  for (byte i = 0; i < bufferSize; i++) {
    Serial.print(buffer[i] < 0x10 ? " 0" : " ");
    Serial.print(buffer[i], HEX);
  }
}