window.interop = {
    dotnetInstance: null,

    registerRazorComponent: function (dotnetRef) {
        this.dotnetInstance = dotnetRef;
    },

    callCSharpMethod: async function (methodName, ...args) {
        if (!this.dotnetInstance) {
            console.error("C# dotnet instance not available.");
            return null;
        }
        try {
            const result = await this.dotnetInstance.invokeMethodAsync(methodName, ...args);

            if (result !== null && result !== undefined) {
                console.log(`Called C# method "${methodName}" successfully.`, result);
                return result;
            } else {
                console.error(`C# method "${methodName}" returned null/undefined`);
                return null;
            }
        } catch (error) {
            console.error(`Error calling C# method "${methodName}":`, error);
            throw error;
        }
    } 
};