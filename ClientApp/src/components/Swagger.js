import SwaggerUI from "swagger-ui-react";
import "swagger-ui-react/swagger-ui.css";

const Swagger = () => {
    return (
        <SwaggerUI url="https:/localhost:5001/swagger/v1/swagger.json" />
    );
}

export default Swagger;