# Testing the application #

Here you can find documentation on how to write and run integration tests locally. 

## Karate tests folder structure ##

* The **karate folder** MUST always be placed in the *project root folder*.  
* The **pom.xml** MUST always be placed in the *karate root folder*. 
* The **KarateRunner.java** file MUST always be placed under the *karate root folder* in the *src/test/java* folder.
* **All other files** MUST always be placed under the *karate root folder* in the *src/test/resources* folder. 
* A **healthchecks.feature** file MUST always be present under the *resources root folder*. 
* If required the **karate-config.js** MUST always be present under the *resources root folder*. 
* **Reuseable features** MUST always be placed under the *resources root folder*.
* **All other feature files** CAN be placed under the *resources root folder* or they CAN be placed under *sepparate subfolders*.
* **All data files** required for the tests MUST be placed under the *resources root folder* in the *data* folder. 

```
karate
|
+-- pom.xml									    !--> ALWAYS HERE <--!
|
\--	src/test
		|
		\-- java
		|	|
		|	\-- KarateRunner.java 			    !--> ALWAYS HERE <--!
		|
		\-- resources
			|
			+-- karate-config.js			    !--> ALWAYS HERE <--!
			+-- healthchecks.feature 		    !--> ALWAYS HERE <--!
			+-- some-reusable.feature		    !--> ALWAYS HERE <--!
			+-- logback-test.xml                !-->  OPTIONAL	<--!
			+-- some-classpath-function.js      !-->  OPTIONAL	<--!
			+-- some-classpath-payload.json     !-->  OPTIONAL	<--!
			+-- some-methodv1.feature           !-->  OPTIONAL	<--!
			+-- some-methodv2.feature           !-->  OPTIONAL	<--!
			|
			\--	endpointA					    !-->  OPTIONAL	<--!
			|	|
			|	+--	some-methodv1.feature
			|	\-- some-methodv2.feature
			|
			\-- endpointB					    !-->  OPTIONAL	<--!
			|	|
			|	+--	some-methodv1.feature
			|	\-- some-methodv2.feature
			|
			\-- data                            !-->  OPTIONAL	<--!
				|
				+-- data.xml
				+-- data.json
				\-- documents
```

## Prerequisites ##

* Have the [application running locally](../README.md).
* [Postman](https://www.getpostman.com/) or any other API test tool.

## Run the tests ##

From the root folder of the project execute: 
```
./mvnw -f karate/pom.xml test -Dtest=karate.KarateRunner 
```
You can also select an environment to run tests against:
```
./mvnw -f karate/pom.xml test -Dtest=karate.KarateRunner -Dkarate.env=dv"
```
If you have Maven installed locally you can simplify the commands.
To run all tests (except @ignore scenario's):
```
mvn test -Dtest=KarateRunner -Dkarate.env=dv
```
To run one specific feature file:
```
mvn test -Dtest=KarateRunner -Dkarate.env=dv -Dkarate.options="path/to/file.feature"
```
To run scenario's with a specified tag (but a tilde (~) in front of the tag to run everything except this tag, eg. ~@myTag):
```
mvn test -Dtest=KarateRunner -Dkarate.env=dv -Dkarate.options="--tags @myTag"
```
## Writing tests ##
Link to the official [karate GitHub page](https://intuit.github.io/karate/) with a lot of documentation. 

### Basics ###

Example SOAP feature file:
```
Feature: Personen

Background:
	* configure logPrettyRequest = true
	* configure logPrettyResponse = true
	* configure ssl = true
	* header Authorization = authz

Scenario: Persoon
	Given url baseUrl
	And def xml_request = read('request_gp.xml')
	And request xml_request
	And print request
	When soap action "urn:GetPerson"
	Then status 200
	And print response
```
Example REST feature file:
```
Feature: Health Checks

Background:
	* configure logPrettyRequest = true
	* configure logPrettyResponse = true
	* configure ssl = true
	* url baseUrl

Scenario: Ask Service Status
	Given path '/status/am-i-up'
	When method GET
	Then status 200
```
Here you see variables like "authz" and "baseUrl" that have been defined in the configuration file because they are environment specific.

The "Background" section is executed before each scenario to set common attributes.

The keywords "Given", "Then", "When", ... come from the Cucumber background of Karate but actually don't have any meaning other than trying to make it more clear what is happening:
* "Given" (with and's) are usually used to define the setup of the request
* "When" performs the action.
* "Then" is used to handle/check the response.

The action is the SOAP action for a SOAP call and the REST VERB (GET,PUT,POST) for a call to a REST API.
