
import { SchoolMasterModel } from "./school-master.model";
import { CommonField } from "./common-field.model";
 
export class schoolDetailsModel {    

    public  id:number;
    public  tenantId:string;
    public  schoolId?:number;    
    public affiliation: string;
    public associations: string;
    public locale: string;
    public lowestGradeLevel: string;
    public highestGradeLevel: string;
    public dateSchoolOpened?: string;
    public dateSchoolClosed?: string;
    public status?: boolean;
    public gender: string;
    public internet?: boolean;
    public electricity?: boolean;
    public telephone: string;
    public fax: string;
    public website: string;
    public email: string;
    public facebook: string;
    public twitter: string;
    public instagram: string;
    public youtube: string;
    public linkedIn: string;
    public nameOfPrincipal: string;
    public nameOfAssistantPrincipal: string;
    public schoolLogo: string;
    public schoolThumbnailLogo: string;
    public runningWater: boolean;
    public mainSourceOfDrinkingWater: string;
    public currentlyAvailable?: boolean;
    public femaleToiletType: string;
    public totalFemaleToilets?: number;
    public totalFemaleToiletsUsable?: number;
    public femaleToiletAccessibility: string;
    public maleToiletType: string;
    public totalMaleToilets?: number;
    public totalMaleToiletsUsable?: number;
    public maleToiletAccessibility: string;
    public comonToiletType: string;
    public totalCommonToilets?: number;
    public totalCommonToiletsUsable?: number;
    public commonToiletAccessibility: string;
    public handwashingAvailable?: boolean;
    public soapAndWaterAvailable?: boolean;
    public hygeneEducation: string;
    public createdBy: string;
    public createdOn: string;
    public updatedBy: string;
    public updatedOn: string;
   
    constructor(){
        this.id= 0;
        this.status=true;
        this.tenantId=JSON.parse(sessionStorage.getItem("tenantId"));
        
    }
    
}


