import { CommonField } from "./common-field.model";

export class GenerateApiKeyModel extends CommonField {    
    apiKeysMaster: ApiKeysMasterModel = new ApiKeysMasterModel();
    constructor(){
        super()
    }
}

export class ApiKeysMasterModel {
    apiTitle: string;
    schoolId: number;
    tenantId: string;
    createdBy: string;
    constructor(){
    }
}


export class GetApiKeyModel extends CommonField {    
    constructor(){
        super()
    }
}

export class UpdateApiKeyModel extends CommonField {    
    apiKeysMaster: ApiKeysMasterModelForUpdate = new ApiKeysMasterModelForUpdate();
    constructor(){
        super()
    }
}

export class ApiKeysMasterModelForUpdate {
    apiTitle: string;
    schoolId: number;
    tenantId: string;
    createdBy: string;
    updatedBy: string;
    keyId: number;
    constructor(){
    }
}

export class ApiAccessmodel extends CommonField {    
    keyId: number;
    constructor(){
        super()
    }
}

export class AddApiAccessmodel extends CommonField {    
    keyId: number;
    apiControllerKeyMapping: ApiControllerKeyMapping[] = []
    constructor(){
        super()
    }
}

export class ApiControllerKeyMapping {    
    controllerId: number;
    isActive: boolean;
    keyId: number;
    updatedBy: string;
    constructor(){
    }
}

