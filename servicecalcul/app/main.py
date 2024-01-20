from pydantic import BaseModel

from typing import List
from fastapi import FastAPI


app = FastAPI()

class Eleve(BaseModel):
    EtudId: str
    Name: str
    S1101: str
    S1102: str
    S1103: str
    S1104: str
    S1105A: str
    S1105B: str
    S1106: str
    S1107: str
    S1108: str
    UE12: str
    BonusUE12: str
    S1201: str
    S1202A: str
    S1202B: str
    S1203: str
    S1204: str
    S1205: str

class Coeff(BaseModel):
    S1101: str
    S1102: str
    S1103: str
    S1104: str
    S1105A: str
    S1105B: str
    S1106: str
    S1107: str
    S1108: str
    UE12: str
    BonusUE12: str
    S1201: str
    S1202A: str
    S1202B: str
    S1203: str
    S1204: str
    S1205: str

class InputData(BaseModel):
    eleves: List[Eleve]
    coeffs: List[Coeff]

def calculer_moyenne_pondere(eleve: Eleve, coeff: Coeff):
    total_somme = 0
    total_coeff = 0

    for key in eleve.dict():
        if key != "EtudId" and key != "Name" and key != "BonusUE12" and key != "UE12":
            total_somme += float(eleve.dict()[key].replace(',','.')) * float(coeff.dict()[key].replace(',','.'))
            total_coeff += float(coeff.dict()[key].replace(',','.'))

    return total_somme / total_coeff


@app.post("/calculmoyenne")
def calculemoyenne(data: InputData):
    print(data)
    
    moyennes_pondere = {}

    for eleve in data.eleves:
        name = eleve.Name
        moyenne = calculer_moyenne_pondere(eleve,data.coeffs[0])
        moyennes_pondere[name] = moyenne

    print(moyennes_pondere)
    return moyennes_pondere
