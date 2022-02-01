export class AvailableTenant {
    public id: number;
    public tenantName: string;
    public tenantId: string;
    public isActive: boolean;
    public tenantFooter: string;
    public tenantLogo: string;
    public tenantLogoIcon: string;
    public tenantFavIcon: string;
    public tenantSidenavLogo: string;    

    constructor() {
        this.id = 0;
        this.tenantName = "";
        this.tenantId = null;
        this.isActive = false;
        this.tenantFooter = '';
        this.tenantLogo = null;
        this.tenantLogoIcon = null;
        this.tenantFavIcon = null;
        this.tenantSidenavLogo = null;

    }
}

export class RestStatus {
    public failure: boolean;
    public message: string;
}

export class AvailableTenantViewModel extends RestStatus {
    public tenant: AvailableTenant;
    constructor() {
        super();
        this.tenant = new AvailableTenant();
    }
}


