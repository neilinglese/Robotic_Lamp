#include <SoftwareSerial.h>
#include <Servo.h>

 
Servo headServo; 
Servo armServo; 
int BTTx = 2; 
int BTRx = 3; 
int led = 13; 

SoftwareSerial BT(BTTx, BTRx); 

void setup()
{
  Serial.begin(9600);
  
  headServo.attach(9);
  armServo.attach(10); 
  
  headServo.write(90);
  armServo.write(15);
  pinMode(led, OUTPUT);

  BT.begin(115200);
  BT.print("$$$");
  delay(100);
  BT.println("U,9600,N");
  BT.begin(9600);
}

void loop()
{
  if(BT.available())
  {
    char GetBT = (char)BT.read();
    Serial.print(GetBT);
    BT.print("Hello");

    if(GetBT == '2')//arm up
    {
      //armServo.write(0);
      for(int pos = 115; pos > 15; pos -=1)
      {
        armServo.write(pos);
        delay(25);   
      }
    }
    if(GetBT == '3')//arm down
    {
      //armServo.write(115);
      for(int pos = 15; pos < 115; pos +=1)
      {
        armServo.write(pos);
        delay(25);   
      }
    }
    if(GetBT == '4')//head left
    {
      headServo.write(180);
    }
    if(GetBT == '5')//head right
    {
      headServo.write(0);
    }
    if(GetBT == '6')//head center
    {
      headServo.write(90);
    }
    if(GetBT == '7')//head sweep
    {
      digitalWrite(led, HIGH);
      for(int pos = 0; pos < 180; pos += 1)  
        {                                 
          headServo.write(pos);             
          delay(15);                      
        }
      for(int pos = 180; pos>=1; pos-=1)    
        {                                
          headServo.write(pos);             
          delay(15);                      
        } 
    }
    if(GetBT == '8')//go to sleep
    {
      digitalWrite(led, LOW);
      headServo.write(90);
      for(int pos = 15; pos < 115; pos +=1)
      {
        armServo.write(pos);
        delay(25);   
      }
    }
    if(GetBT == '9')//wake up
    {
      digitalWrite(led, HIGH);
      for(int pos = 115; pos > 0; pos -=1)
        {
          armServo.write(pos);
          delay(25);   
        }
      for(int pos = 0; pos < 180; pos += 1)  
        {                                 
          headServo.write(pos);             
          delay(15);                      
        }
      for(int pos = 180; pos>=1; pos-=1)    
        {                                
          headServo.write(pos);             
          delay(15);                      
        } 
    }
    if(GetBT == '0')//led on
    {
      digitalWrite(led, HIGH);
    }
    if(GetBT == '1')//led off
    {
      digitalWrite(led, LOW);
    }
  }
}
