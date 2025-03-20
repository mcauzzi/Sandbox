export interface RoutingMenuItem {
    path:string;
    label:string;
    command?: () => void;
}
